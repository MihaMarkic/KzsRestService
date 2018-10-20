var page = require('webpage').create(),
system = require('system');
var base64js = require("./base64.min.js");
var te = require("./text-encoder-lite.js");

var currentOperation = 0;

page.onLoadFinished = function() {
	//console.log('if (secondRound === false): ' + secondRound === false);
	//console.log("Load finished on iteration ", currentOperation);
	if (currentOperation === 0)
	{
		console.log("#Root#");
	}
	else
	{
		console.log("#" + system.args[1+currentOperation] + "#")
	}
	console.log(Base64Encode(page.content), "#");
    //console.log("Page dump");
	if (currentOperation < system.args.length-2)
	{
		currentOperation++;
		//console.log("evaluating");
		//console.log("**** Clicking");
		var id = system.args[1+currentOperation];
		//console.log("Clicking id ", id);
        page.evaluate(function (id) {
            
			var el = document.getElementById(id);
			if (el === null)
			{
				//console.log("id not found");
			}
			else
			{
				//console.log("Clicking");
				el.click();
			}
		}, id);
		// console.log("elx == null: " + elx);
		// $(elx).click();
		//console.log("***** clicked");
		//console.log(Base64Encode(page.content));
		//console.log(page.content);
	}
	else
	{
		//console.log("Done on operation ", currentOperation);
		//console.log(Base64Encode(page.content));
		phantom.exit();
	}
};

//page.onConsoleMessage = function (msg) {
//    system.stderr.writeLine('console: ' + msg);
//};
// page.onLoadFinished = function(status) {
//     console.log('Load Finishedx: ' + status);
// };
// page.onResourceReceived = function(response) {
//     if (response.stage !== "end") return;
// 	//console.log('Response (#' + response.id + ', stage "' + response.stage + '"): ' + response.url);
// 	if (response.url.startsWith('https://widgets.baskethotel.com/widget-service/'))
// 	//if (response.url.startsWith('https://185.38.167.137/'))
// 	{
// 		//console.log(Base64Encode(page.content));
// 		console.log("Done");
// 		//console.log(page.content);
// 		isDone = true;
// 	}
// };
// page.onResourceRequested = function(requestData, networkRequest) {
//     //console.log('Request (#' + requestData.id + '): ' + requestData.url);
// };
// page.onUrlChanged = function(targetUrl) {
//     //console.log('New URL: ' + targetUrl);
// };
// page.onLoadStarted = function() {
//     //console.log('Load Started');
// };
// page.onNavigationRequested = function(url, type, willNavigate, main) {
//     //console.log('Trying to navigate to: ' + url);
// };

// var test = {
// 	url: 'http://www.kzs.si/incl?id=967&team_id=195883&league_id=undefined&season_id=102583',
// 	items: [
// 		{
// 			click: null,
// 			nodes: ['33-200-qualizer-1', '']
// 		}
// 	]
// };

page.open(system.args[1]);

function Base64Encode(str) {
	var bytes = new (te.TextEncoderLite)('utf-8').encode(str);        
	return base64js.fromByteArray(bytes);
};
