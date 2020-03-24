mergeInto(LibraryManager.library, {

  // errors in this file can be debugged much more eaisly by following the instructions at https://github.com/kripken/emscripten/issues/5700#issuecomment-387684922

  InitializeWebSocket: function(host_cs, port_cs, id_cs) {
    var host=Pointer_stringify(host_cs);
    var port=Pointer_stringify(port_cs);
    var id=Pointer_stringify(id_cs);

    if (! host) {
      host = location.hostname;
    }
    var address = 'ws://'+host+':'+port+'/dump/'+id;

    var self = this;
    function init() {
      console.log("WebSocket connecting to", address);
      self.ws = new WebSocket(address);

      self.ws.addEventListener("close", function() {
        setTimeout(init, 500);
      });      
    }

    init();
  },

  IsBufferOk: function() {
    if (this.ws.bufferedAmount < 5000000) { // less than 5 MB in buffer
      return true;
    } else {
      return false;
    }
  },

  SendDataToWebSocketString: function(str) {
    if (this.ws.readyState === 1) {
      this.ws.send(Pointer_stringify(str));
      return true;
    } else {
      return false;
    }
  },
    
  SendDataToWebSocketArray: function(array, size) {
    var buffer = HEAPU8.buffer.slice(array, array + size);
    //console.log(buffer);
    if (this.ws.readyState === 1) {
      this.ws.send(buffer);
      return true;
    } else {
      return false;
    }
  },

  CloseWebSocket: function() {
    this.ws.close();
  },
  
  WebSocketIsUp: function() {
    return this.ws.readyState === 1;
  },
  
  TaskHasCompleted: function() {
    alert("Done!");
    window.location = "about:blank";
  },
  
  GetParameterData: function(key_cs) {
    var key = Pointer_stringify(key_cs);
    
    var data = Number(location.pathname.substr(1) || 0);

    switch (key) {
    case 'height':
      return data & 0x3;
    case 'speed':
      return (data >> 2) & 0x3;
    case 'key':
      return data;
    }
    
    return -1;
  },

});
