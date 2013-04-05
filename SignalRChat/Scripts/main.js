function setBoxLocation(box) {
    $('#' + box.Name).css('left', box.X + 'px').css('top', box.Y + 'px').css('backgroundColor', '#' + box.Color);
}

function createBox(box) {
    // may get passed as a dictionary object so this is the work around
    box = box.Value || box;
    var newbox = $('<div></div>').attr('id', box.Name).addClass('box');
    $('body').prepend(newbox);
    setBoxLocation(box);
}

function setDragable(box) {
    var target = $('#' + box.Name);
    target.draggable({
        drag: dragBoxEvent
    });
    target.addClass('movable');
}

function dragBoxEvent(event, ui) {
    // Call the MoveBox method on the hub. 
    var box = {
        Name: ui.helper.context.id,
        Color: $('#color').val(),
        X: ui.offset.left,
        Y: ui.offset.top
    };
    hub.server.moveBox(box);
}

$(document).ready(function() {
    // Add script to update the page and send messages.
    $('#color').ColorPicker({
        color: '#FF0000',
        onChange: function (hsb, hex, rgb) {
            var color = $('#color');
            color.val(hex);
            color.css('border', '1px solid #' + hex);
            var $target = $('.movable');
            $target.css('backgroundColor', '#' + hex);
            // trigger drag event to send update to SignalR
            var box = {
                Name: $target[0].id,
                Color: hex,
                X: $target[0].offsetLeft,
                Y: $target[0].offsetTop
            };
            hub.server.moveBox(box);
        }
    });
    
    
    // Declare a proxy to reference the hub. 
    var hub = $.connection.chatHub;
    window.hub = hub;

    // fires whenever there's a new connection
    hub.client.joined = function(box) {
        createBox(box);
    };

    // fires whenever a connection is closed
    hub.client.leave = function(boxName) {
        $('#' + boxName).remove();
    };

    // Create a function that the hub can call to update box location.
    hub.client.updateBoxLocation = function (box) {
        var target = $('#' + box.Name);
        if (target.length === 0) {
            createBox(box);
        } else {
            setBoxLocation(box);
        }
    };

    // boxes is a dictionary object
    hub.client.createOwnBox = function (box, boxes) {
        createBox(box);
        setDragable(box);

        for (var i = 0; i < boxes.length; i++) {
            if (box.Name !== '' && box.Name !== boxes[i].Value.Name) {
                createBox(boxes[i]);
            }
        }
    };
    
    // Start the connection.
    $.connection.hub.start().done(function () {
        
    });
});