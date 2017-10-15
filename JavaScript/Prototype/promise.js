var Promise = function(){
    var data, done = [], fail = [], status = 'progress';
    this.done = function(fn){
        done.push(fn);
        
        if(status === 'done')
            fn(data);
        
        return this;
    };
    this.failed = function(fn){
        fail.push(fn);
        
        if(status === 'failed'){
            fn(data);
        }
        
        return this;
    };
    this.resolve = function(result){
        if(status !== 'progress'){
            throw "Promise has already complete with a status of ' + status + ' and cause exception.";
        }
        
        status = 'done';
        data = result; 
        for(var i=0; i<done.length; i++){
            done[i](data);
        }
    };
    this.fail = function(reason){
           if(status !== 'progress'){
            throw "Promise has already complete with a status of ' + status + ' and cause exception.";
        }
        
        status = 'failed';
        data = reason;
        for(var i=0; i< fail.length; i++){
            fail[i](data);
        }
        
        return this;
    };
};

module.exports = Promise;