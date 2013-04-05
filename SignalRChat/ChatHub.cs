using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Runtime.Caching;

namespace SignalRChat
{
    public class Box
    {
        public string X { get; set; }
        public string Y { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }
    }

    public class ChatHub : Hub
    {
        private static string boxCacheName = "Boxes";
        private static MemoryCache cache = MemoryCache.Default;
 
        public ChatHub()
        {
            
        }

        private void removeBox(string name)
        {
            var boxes = getBoxCache();
            boxes.Remove(name);
        }

        private void addBox(Box box)
        {
            var boxes = getBoxCache();
            boxes.Add(box.Name, box);
        }

        private IDictionary<string,Box> getBoxCache()
        {
            if (cache[boxCacheName] == null)
                cache[boxCacheName] = new Dictionary<string, Box>();

            return cache[boxCacheName] as Dictionary<string,Box>;
        } 

        public override Task OnConnected()
        {
            var box = new Box
                {
                    Name = Context.ConnectionId,
                    Color = "FF0000",
                    X = "50",
                    Y = "50"
                };


            addBox(box);

            Clients.Others.joined(box);
            return Clients.Caller.createOwnBox(box, getBoxCache().ToList());
        }

        public override Task OnDisconnected()
        {
            removeBox(Context.ConnectionId);
            return Clients.Others.leave(Context.ConnectionId);
        }

        public void MoveBox(Box box)
        {
            // Update box value
            var boxes = getBoxCache();
            boxes[box.Name] = box;
            // Call the updateBoxLocation method to update everyone but the sender.
            Clients.Others.updateBoxLocation(box);
        }

        
    }
}