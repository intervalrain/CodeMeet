using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Text.Json;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Domain;
using CodeMeet.Ddd.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeMeet.Infrastructure.Common.Persistence.JsonFile;

/// <summary>
/// JSON file-based repository with in-memory cache.
/// Data is loaded from disk on first access and cached in memory.
/// Changes are persisted to disk via UnitOfWork.SaveChangesAsync().
/// </summary>
public class JsonFileRepository<TAggregateRoot, TId> : IRepository<TAggregateRoot, TId>, IJsonFilePersistable
    where TAggregateRoot : AggregationRoot<TId>
    where TId : notnull
{
    private readonly ConcurrentDictionary<TId, TAggregateRoot> _cache = new();
    private readonly string _filePath;
    private readonly PersistenceOptions _options;
    private readonly ILogger<JsonFileRepository<TAggregateRoot, TId>> _logger;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _isLoaded;
    private bool _isDirty;

    public bool IsDirty => _isDirty;


    public JsonFileRepository(
        IOptions<PersistenceOptions> options,
        ILogger<JsonFileRepository<TAggregateRoot, TId>> logger,
        JsonFileRepositoryRegistry registry)
    {
        _options = options.Value;
        _logger = logger;

        var entityName = typeof(TAggregateRoot).Name.ToLowerInvariant();
        _filePath = Path.Combine(_options.DataDirectory, $"{entityName}.json");

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = _options.IndentedJson,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        };

        registry.Register(this);
    }

    private async Task EnsureLoadedAsync(CancellationToken token = default)
    {
        if (_isLoaded) return;

        await _lock.WaitAsync(token);
        try
        {
            if (_isLoaded) return;

            if (File.Exists(_filePath))
            {
                var json = await File.ReadAllTextAsync(_filePath, token);
                var entities = JsonSerializer.Deserialize<List<TAggregateRoot>>(json, _jsonOptions);

                if (entities != null)
                {
                    foreach (var entity in entities)
                    {
                        _cache.TryAdd(entity.Id, entity);
                    }
                }

                _logger.LogDebug("Loaded {Count} {EntityType} from {FilePath}",
                    _cache.Count, typeof(TAggregateRoot).Name, _filePath);
            }
            else
            {
                _logger.LogDebug("No existing data file found at {FilePath}, starting fresh",
                    _filePath);
            }

            _isLoaded = true;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task PersistAsync(CancellationToken token = default)
    {
        if (!_isDirty) return;

        await _lock.WaitAsync(token);
        try
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var entities = _cache.Values.ToList();
            var json = JsonSerializer.Serialize(entities, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json, token);

            _isDirty = false;

            _logger.LogDebug("Saved {Count} {EntityType} to {FilePath}",
                entities.Count, typeof(TAggregateRoot).Name, _filePath);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<TAggregateRoot?> FindAsync(TId id, CancellationToken token = default)
    {
        await EnsureLoadedAsync(token);
        _cache.TryGetValue(id, out var entity);
        return entity;
    }

    public async Task<TAggregateRoot> GetAsync(TId id, CancellationToken token = default)
    {
        await EnsureLoadedAsync(token);
        if (!_cache.TryGetValue(id, out var entity))
            throw new KeyNotFoundException($"Entity with id {id} not found");

        return entity;
    }

    public async Task<IReadOnlyList<TAggregateRoot>> GetListAsync(CancellationToken token = default)
    {
        await EnsureLoadedAsync(token);
        return _cache.Values.ToList();
    }

    public async Task<TAggregateRoot?> FindAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
    {
        await EnsureLoadedAsync(token);
        var compiled = predicate.Compile();
        return _cache.Values.FirstOrDefault(compiled);
    }

    public async Task<IReadOnlyList<TAggregateRoot>> GetListAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
    {
        await EnsureLoadedAsync(token);
        var compiled = predicate.Compile();
        return _cache.Values.Where(compiled).ToList();
    }

    public async Task<int> CountAsync(Expression<Func<TAggregateRoot, bool>>? predicate = null, CancellationToken token = default)
    {
        await EnsureLoadedAsync(token);
        return predicate == null
            ? _cache.Count
            : _cache.Values.Count(predicate.Compile());
    }

    public async Task<bool> AnyAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
    {
        await EnsureLoadedAsync(token);
        var compiled = predicate.Compile();
        return _cache.Values.Any(compiled);
    }

    public IQueryable<TAggregateRoot> AsQueryable()
    {
        EnsureLoadedAsync().GetAwaiter().GetResult();
        return _cache.Values.AsQueryable();
    }

    public async Task<PaginationResult<TAggregateRoot>> GetPagedListAsync(IPaginationQuery pagination, CancellationToken token = default)
    {
        await EnsureLoadedAsync(token);
        var totalCount = _cache.Count;
        var entities = _cache.Values
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList();

        return new PaginationResult<TAggregateRoot>(pagination.PageNumber, pagination.PageSize, totalCount, entities);
    }

    public async Task<PaginationResult<TAggregateRoot>> GetPagedListAsync(Expression<Func<TAggregateRoot, bool>> predicate, IPaginationQuery pagination, CancellationToken token = default)
    {
        await EnsureLoadedAsync(token);
        var compiled = predicate.Compile();
        var filtered = _cache.Values.Where(compiled).ToList();
        var totalCount = filtered.Count;
        var entities = filtered
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList();

        return new PaginationResult<TAggregateRoot>(pagination.PageNumber, pagination.PageSize, totalCount, entities);
    }

    public async Task InsertAsync(TAggregateRoot entity, CancellationToken token = default)
    {
        await EnsureLoadedAsync(token);
        _cache.TryAdd(entity.Id, entity);
        _isDirty = true;
    }

    public async Task UpdateAsync(TAggregateRoot entity, CancellationToken token = default)
    {
        await EnsureLoadedAsync(token);
        _cache[entity.Id] = entity;
        _isDirty = true;
    }

    public async Task DeleteAsync(TAggregateRoot entity, CancellationToken token = default)
    {
        await EnsureLoadedAsync(token);
        _cache.TryRemove(entity.Id, out _);
        _isDirty = true;
    }
}

public class JsonFileRepository<TAggregateRoot>(
    IOptions<PersistenceOptions> options,
    ILogger<JsonFileRepository<TAggregateRoot, Guid>> logger,
    JsonFileRepositoryRegistry registry) : JsonFileRepository<TAggregateRoot, Guid>(options, logger, registry), IRepository<TAggregateRoot>
    where TAggregateRoot : AggregationRoot<Guid>
{
}
