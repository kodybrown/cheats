#!/usr/bin/env node

// var prompt = require('prompt');
var program = require('commander');
var request = require('request');
var fs = require('fs');

//program.url = "https://raw.github.com/kodybrown/cheats/master/files/";

program.version('0.0.1')
       //.option('--debug, --verbose', 'Displays detailed information during run.')
       .option('-n, --noheader', 'Display plain output, no header info, etc.')
       .option('-f, --force', "Force download of cheat sheet, whether it is local already or not.")
       .option('-l, --list', "List the local cheat sheets")
       .option('-s, --server', 'List the cheat sheets on the server')
       .option('-u, --url [url]', "Override the default file repository", "https://raw.github.com/kodybrown/cheats/master/files/")
       .option('-d, --remove [sheet]', "Remove a local cheat sheet.", "")
    .parse(process.argv);
// `program.args` is an array of everything entered that is not a flag..
//console.log('  - %s list', program.list);

program.sheetPath = "C:/Users/Kody/AppData/Roaming/.cheats";


if (program.list) {
    displaySheets(true);
} else if (program.server) {
    displaySheets(false);
} else if (program.remove) {
    removeSheet(program.remove);
} else if (program.args && program.args.length > 0) {
    displaySheet(program.args[0]);
}


function displaySheet( sheet )
{
    if (program.force || !fs.existsSync(program.sheetPath + "\\" + sheet)) {
        downloadSheet(sheet, function (sheet) {
            outputSheet(sheet);
        });
    } else {
        outputSheet(sheet);
    }
}

function outputSheet( sheet )
{
    fs.readFile(program.sheetPath + "\\" + sheet, { encoding: "utf-8" }, function(err, data) {
        if (err)
            throw err;
        // console.log("");
        console.log(data);
    });
}

function downloadSheet( sheet, callback )
{
    request(program.url + sheet, function (error, response, body) {
        if (!error && response.statusCode == 200) {
            fs.writeFile(program.sheetPath + "\\" + sheet, body, { encoding: "utf-8" }, function (err) {
                if (err)
                    throw err;
                console.log("Successfully saved sheet to disk!\n");
                if (callback) {
                    callback(sheet);
                }
            });
        }
    });
}

function displaySheets( showLocal )
{
    var padding;

    if (program.noheader) {
        padding = "";
    } else {
        padding = "  ";
    }

    if (showLocal) {
        if (!program.noheader) {
            console.log("Showing local sheets:");
            console.log(program.sheetPath);
            console.log("");
        }
        fs.readdir(program.sheetPath, function (err, files) {
            for (var i = 0; i < files.length; i++) {
                var f = files[i].toLowerCase();
                if (f == "desktop.ini" || f == "thumbs.db") {
                    continue;
                }
                console.log(padding + files[i]);
            }
        });
    } else {
        if (!program.noheader) {
            console.log("Showing sheets available on server:");
            console.log(program.url);
            console.log("Sheets marked with a * exist locally.");
            console.log("");
        }
        request(program.url, function (error, response, body) {
            if (!error && response.statusCode == 200) {
                var lines = body.split("\n");
                for (var i = 0; i < lines.length; i++) {
                    var l = lines[i];
                    if (l.indexOf("js-directory-link") > -1) {
                        // The line ends with this: ">aes</a></span>"
                        l = l.substr(0, l.length - "</a></span>".length);
                        l = l.substr(l.lastIndexOf('>') + 1);
                        if (!program.noheader && fs.existsSync(program.sheetPath + "\\" + l)) {
                            isLocal = "*"
                        } else {
                            isLocal = ""
                        }
                        console.log(padding + l + isLocal);
                    }
                }
            }
        });
    }
}

function removeSheet( sheet )
{
    if (fs.existsSync(program.sheetPath + "\\" + sheet)) {
        fs.unlink(program.sheetPath + "\\" + sheet, function (err) {
            if (err)
                throw err;
            console.log("Successfully removed " + sheet);
        });
    } else {
        console.log("The specified sheet does not exist locally.");
    }
}

