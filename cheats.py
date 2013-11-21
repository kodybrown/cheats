#!/usr/bin/env python

# cheat.py

__author__ = "Kody Brown (kody@bricksoft.com)"
__created__ = "10/23/2013"
__copyright__ = "(C) 2013 Kody Brown. Released under the MIT License."
__contributors__ = ["Kody Brown"]
__version__ = "0.21"

import glob
import os
import sys
import shutil, errno
from urllib.request import urlopen

__debug = False

sheetPath = ""
downloadUrls = []

def debug( a, b = "", c = "", d = ""):
    global __debug
    if __debug:
        if len(str(b)) > 0:
            if a != None:
                print("DEBUG:", a.ljust(14, ' ') + " =", b, c, d)
            else:
                print("DEBUG:", ' '.ljust(14, ' ') + " =", b, c, d)
        else:
            print("DEBUG:", a)


def getSheetPath():
    try:
        path = os.environ["AppData"]
    except:
        path = None
    if path == None or len(path.strip()) == 0:
        return "."
    else:
        p = os.path.join(path, ".cheats")
        if os.path.exists(p) and not os.path.isdir(p):
            raise ValueError("Could not create directory: a file of the same name is in the way")
        if not os.path.isdir(p):
            os.makedirs(p)
        return p

def getDownloadUrls():
    return [
        "https://raw.github.com/kodybrown/cheats/master/files/"
    ]

def validateFile( filename, forceDownload ):
    global __debug, sheetPath, downloadUrls

    destfile = os.path.join(sheetPath, filename)
    debug('sheetPath', sheetPath)
    debug('filename', filename)
    debug('destfile', destfile)

    if forceDownload or not os.path.exists(destfile):
        for d in downloadUrls:
            url = d + filename
            debug('url', url)
            if downloadFile(url, destfile):
                break

    if not os.path.exists(destfile):
        print("could not find sheet..")
        return False
    else:
        return True

def downloadFile( url, filename ):
    global __debug, sheetPath, downloadUrls

    if __debug:
        print("saving '" + url + "'\r\n    to '" + filename + "'")

    try:
        ret = urlopen(url)
        if ret.code == 200:
            output = open(filename, 'wb')
            output.write(ret.read())
            output.close()
            if __debug:
                print("successfully downloaded file")
            return True
        else:
            print("failed to download file: " + str(ret.code))
            return False
    except:
        print("failed to download file: an exception occurred")
        return False

def usage():
    print("cheat.py - command-line cheat sheets, that works in Windows.")
    print("")
    print("USAGE:")
    print("    cheat.py [options] name")
    print("")
    print("    name           The name of the cheet sheat to view.")
    print("                   If it does not already exist in the local cache it will be downloaded then displayed.")
    print("")
    print("OPTIONS:")
    print("    --debug        Outputs (a lot of) additional details about what is going on, etc.")
    #print("    --download     Forces download even if it already exists locally.")

def main():
    global __debug, sheetPath, downloadUrls

    sheetPath = getSheetPath()
    downloadUrls = getDownloadUrls()
    debug("sheetPath", sheetPath)
    debug("downloadUrls", downloadUrls)

    forceDownload = False
    args = []

    if len(sys.argv) < 2:
        usage()
        sys.exit()

    for a in sys.argv[1:]:
        if len(a.strip()) == 0:
            continue
        debug("arg", a)

        if a[:1] in ("-", "/", "!"):
            while a[:1] in ("-", "/"):
                a = a[1:]
            al = a.lower()
            if al in ("?", "help"):
                usage()
                sys.exit(0)
            elif al == "debug":
                __debug = True
            elif al in ("download", "force"):
                forceDownload = True
            elif al in ("!download", "!force"):
                forceDownload = False
        else:
            if a is not None and len(a) > 0:
                args.append(a)

    if __debug:
        debug("forceDownload", forceDownload)
        debug("args", args)

    if not validateFile(args[0], forceDownload):
        sys.exit(1)

    destfile = os.path.join(sheetPath, args[0])
    # with open(destfile, "r") as f:
    #     content = f.read()
    # # TODO fancy print the file's content..
    # print(content)

    # my cat utility already handles outputting files via cli..
    # https://github.com/kodybrown/cat
    os.system("cat -w " + destfile)

    sys.exit(0)

if __name__ == "__main__":
    main()
