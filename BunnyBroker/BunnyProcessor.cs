using BunnyBroker.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace BunnyBroker;

public class BunnyProcessor(IHubContext<BunnyHub, IBunnyObserver> hubContext) {
	private readonly List<ConnectionHandlerName> _dic = new();

	public void Add(string bunnyType, string connectionId, string handlerName) {
		_dic.Add(
			new ConnectionHandlerName {
				ConnectionId = connectionId,
				HandlerName = handlerName,
				BunnyType = bunnyType
			});
	}

	public void Remove(string connectionId) {
		_dic.RemoveAll(x => x.ConnectionId == connectionId);
	}

	public async Task Process(BunnyMessage message) {
		var request = new BunnyMessageProcessRequest() {
			Message = message
		};
		var multipleHandlers = _dic.Where(x => x.BunnyType == message.BunnyType)
			.GroupBy(x => x.HandlerName)
			.Where(x => x.Any())
			.ToDictionary(x => x.Key, x => x.DistinctBy(y=>y.ConnectionId).ToList());

		if (multipleHandlers.Any()) {
            var excluded = new List<BunnyObserverInfo>();
            foreach (var connectionHandlerNames in multipleHandlers.Values) {
	            int index = Random.Shared.Next(connectionHandlerNames.Count);
	            excluded.AddRange(
		            connectionHandlerNames.Where(x => connectionHandlerNames.IndexOf(x) != index)
			            .Select(x => new BunnyObserverInfo {
							ConnectionId = x.ConnectionId,
							HandlerName = x.HandlerName
			            }));

            }
			request.ExcludedObservers = excluded;
        }

        await hubContext.Clients.Groups(message.BunnyType).OnBunnyReceivedAsync(request);
    }


	private class ConnectionHandlerName {
		public string ConnectionId { get; set; }
		public string HandlerName { get; set; }
		public string BunnyType { get; set; }
	}
}

