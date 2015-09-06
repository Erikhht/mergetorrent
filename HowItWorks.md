# The basics #

## About torrents ##

A .torrent file is a small file that has information in it about which files are available to download, the file names, the length of each file and a bunch of other stuff.  It also has a _[cryptographic hash](http://en.wikipedia.org/wiki/Cryptographic_hash_function)_ of each _piece_ of the torrent.  A piece is just a slice of one of the files in the torrent.  The torrent's files are divided up into pieces so that you don't need to download the entire torrent before you check that the data is correct.

## About hashes ##

A hash is complicated mathematical function to check the if contents of a file are correct.  The hash function reads the input file and outputs a number.  If the output number matches the one in the .torrent file then the file has been downloaded correctly.  If not then either the download was corrupt or that file has been received yet by uTorrent.  Torrents hash _pieces_, **not** files.

# The details #

When mergetorrent runs, for each .torrent file it collects information about all the files needed and the size of each file for a torrent.  Then it searches through all the torrents, files and directories specified for files that are the right size.  All those files might help complete your download.

Finally, mergetorrent starts at the first file of the torrent and tries to complete the first piece.  It tries all the possible files until it finds the piece that outputs the right hash value.  If mergetorrent finds a valid piece it saves the correct data in the file and moves on to the next piece.  Sometimes mergetorrent has to try files from a few different possibilities but it tries all the combinations before giving up and moving on to the next piece.

That's it!  mergetorrent doesn't want to mess with uTorrent's internal storage (mergetorrent never alters uTorrent's internal files) so it's up to the user to "Force re-check" in uTorrent.