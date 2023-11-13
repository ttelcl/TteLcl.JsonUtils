# jsonnormalize

Utility to normalize a JSON file, reordering properties in
objects to be sorted alphabetically.

## Synopsis

```
jsonnormalize -f <file>.json -o <outfile>.json
jsonnormalize -f <file>.json
jsonnormalize -f <file>.json -inplace 
```

Reads the input file given by the `-f` option, reorders all object
properties to be in alphabetical order and writes the result to
the output file.

In the first form the output file is named explicitly by the `-o`
option.

In the second form the output file name is derived from the input
file name.

In the third form, the output file has the same name as the input.

In all cases, if the output file already exists, the existing file
is renamed to have the `.bak` extension.

