using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

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
        private IList<Box> boxes;
 
        public ChatHub()
        {
            boxes = new List<Box>();
        }

        private void removeBox(string name)
        {
            foreach (Box box in boxes)
            {
                if (box.Name == name)
                {
                    boxes.Remove(box);
                    break;
                }
            }
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

            boxes.Add(box);

            Clients.Others.joined(box);
            return Clients.Caller.createOwnBox(box, boxes);
        }

        public override Task OnDisconnected()
        {
            removeBox(Context.ConnectionId);
            return Clients.Others.leave(Context.ConnectionId);
        }

        public void MoveBox(Box box)
        {
            // Call the updateBoxLocation method to update everyone but the sender.
            Clients.Others.updateBoxLocation(box);
        }

        
    }
}