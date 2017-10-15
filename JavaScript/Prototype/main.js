//subject
var Task = function(name, user, description){
    this.name = name;
    this.user = user;
    this.description = description;
}

//observer.
var NotificationService = function(){
    var message = "notifying";
    this.update = function(task){
        console.log(message + task.user + " for task " + task.name);
    }
}

function ObserverList(){
    this.objserverList = [];
    
}
ObserverList.prototype.add = function(obj){
    return this.objserverList.push(obj);
}
ObserverList.prototype.get = function(index){
    if(index <= -1 || index >= this.objserverList.length)
        return null;
        
    return this.objserverList[index];
}
ObserverList.prototype.length = function(){
    return this.objserverList.length;
}

var ObservableTask = function(data){
    Task.call(this, data);
    this.observerList = new ObserverList();
}
ObservableTask.prototype = Object.create(Task.prototype);
ObservableTask.prototype.addObserver = function(observer){
    this.observerList.add(observer);
}
ObservableTask.prototype.notify = function(context){
    var observerCount = this.observerList.length(); 
    console.log(observerCount);
    for(var i=0; i< observerCount; i++){
        this.observerList.get(i)(context);
    }
}
ObservableTask.prototype.save = function(){
    this.notify(this);
}

var mediator= (function(){
    var channels ={};
    var subscribe = function(channel , context, func){
        if(!mediator.channels[channel]){
            mediator.channels[channel] = [];
        }    
        
        mediator.channels[channel].push({
            context: context,
            func: func
        });
    }
    var publish = function(channel){
        if(!this.channels[channel]){
            return false;
        }
        
        var args = Array.prototype.splice.call(arguments, 1);
        for(var i=0; i< mediator.channels[channel].length; i++){
            var sub = mediator.channels[channel][i];
            sub.func.apply(sub.context, args);
        }
    }
    
    return {
        channels: {},
        subscribe: subscribe,
        publish: publish
    }
}());

var notify = new NotificationService();
var task1 = new Task("test 1","user1", "test");
Task.prototype.complete=function(){
    console.log("task " + this.name + " was saved");
}

mediator.subscribe("complete", notify, notify.update);
task1.complete = function(){
    mediator.publish("complete", this);
    Task.prototype.complete.call(this);
}
task1.complete();