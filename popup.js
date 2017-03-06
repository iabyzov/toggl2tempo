var fetch = require('node-fetch');

var myInit = {
    method: 'GET',
    //headers: {Authorization: 'Basic ' + new Buffer('login' + ':' + 'pass').toString('base64')},
    headers: {Authorization: 'Basic ' + new Buffer('' + ':api_token').toString('base64')},
    compress: false
}

fetch('https://www.toggl.com/api/v8/time_entries', myInit)
    .then(function(res) {
        return res.json();
    }).then(function(body) {
        console.log(body);
    });
