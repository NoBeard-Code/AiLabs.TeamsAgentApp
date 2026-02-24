using System.Collections.Concurrent;

namespace AiLabs.MsTeamsAdapter.Configuration;

/// <summary>
/// Represents a registry that manages service instances identified by unique Guid keys.
/// </summary>
/// <remarks>Implementations of this interface allow adding, retrieving, and removing service instances by key.
/// Keys are typically case-sensitive and must be unique within the registry. This interface is commonly used to support
/// scenarios where multiple named or keyed services of the same type are required.</remarks>
/// <typeparam name="T">The type of service instance to be registered and retrieved.</typeparam>
public interface IKeyedServiceRegistry<T>
{
    void Add(Guid key, T instance);

    bool TryGet(Guid key, out T instance);

    bool Remove(Guid key);

    IReadOnlyCollection<Guid> Keys { get; }
}

/// <summary>
/// Provides a thread-safe registry for associating string keys with service instances of a specified type.
/// </summary>
/// <remarks>This class enables registration, retrieval, and removal of services by Guid key. It is designed for
/// scenarios where multiple named or keyed service instances of the same type need to be managed. All operations are
/// thread-safe and suitable for concurrent access.</remarks>
/// <typeparam name="T">The type of service instances to be registered and retrieved.</typeparam>
public class KeyedServiceRegistry<T> : IKeyedServiceRegistry<T>
{
    private readonly ConcurrentDictionary<Guid, T> _services = new();

    public void Add(Guid key, T instance) => _services[key] = instance;

    public bool TryGet(Guid key, out T instance) => _services.TryGetValue(key, out instance);

    public bool Remove(Guid key) => _services.TryRemove(key, out _);

    public IReadOnlyCollection<Guid> Keys => _services.Keys.ToList();
}
