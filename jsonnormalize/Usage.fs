// (c) 2023  ttelcl / ttelcl
module Usage

open CommonTools
open ColorPrint

let usage detail =
  cp "\foUtility to normalize a JSON file, reordering properties in objects to be sorted alphabetically.\f0"
  cp ""
  cp "\fojsonnormalize \fg-f\f0 <\fcfile.json\f0>"
  cp "\fojsonnormalize \fg-f\f0 <\fcfile.json\f0> \fg-o\f0 <\fcoutput.json\f0>"
  cp "\fojsonnormalize \fg-f\f0 <\fcfile.json\f0> \fg-inplace\f0"
  cp ""
  cp "\fg-v               \f0Verbose mode"



