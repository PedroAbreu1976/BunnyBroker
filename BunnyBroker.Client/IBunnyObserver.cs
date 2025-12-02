namespace BunnyBroker.Client;

public interface IBunnyObserver {
	Task<bool> BunnyPublished(object bunny, Guid id);
}

public interface IBunnyObserver<in TBunny> : IBunnyObserver {
	Task<bool> BunnyPublished(TBunny bunny, Guid id) => BunnyPublished((object)bunny!, id);

	Task<bool> IBunnyObserver.BunnyPublished(object bunny, Guid id) => BunnyPublished((TBunny)bunny, id);
}

