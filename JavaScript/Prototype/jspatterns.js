//Function
function add(a, b, c){
    return a + b + c;
}

console.log(add(1,2,3));
console.log(add(1,2,3,4));
console.log(add(1,2));



//Chaining 
console.log("===================Chaining==========================")
var Calc = require("./calculator");
new Calc(0)
.add(1)
.add(2)
.multiply(3)
.equals(function(result){
    console.log(result);
})

//ES5 Prooperties
console.log("===================ES5 Properties==========================")
function Book(){
    var name = "";
    Object.defineProperty(this, 'name', {
        get: function(){
            return name;
        },
        set: function(val){
            console.log(val);
            name = val;
        }
    })
}

var book = new Book();
book.name = "Advanced JavaScript";

//Promise
var Promise = require('./promise');
var promise = new Promise();

setTimeout(function(){
    promise.resolve();
}, 1000)

setTimeout(function(){
    promise.done(function(data){
        console.log("Handler added after deffered object is done.");
    });
}, 2000);

promise.done(function(data){
    console.log("Deferred object has completed.");
});

var promise2 = new Promise();
promise2.failed(function(){
   console.log("Promise #2 failed"); 
}).done(function(){
    console.log("Promise #2 has completed.")
});

setTimeout(function(){
    promise2.fail();
}, 1000);


