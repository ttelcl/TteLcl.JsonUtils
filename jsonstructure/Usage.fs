// (c) 2025  ttelcl / ttelcl
module Usage

open CommonTools
open ColorPrint

let usage focus =
  cp "\fojsonstructure - JSON file structure analysis tool\f0"
  cp ""
  cp "\fojsonstructure \fyanalyze \fg-f \fcinput.json \f0[\fg-o \fcoutput.json\f0]"
  cp "   Analyze the input file and write a description of the structure to the output"
  cp ""
  cp "\fojsonstructure \fyobjecttypes \fg-f \fcinput.json \f0"
  cp "   List the distinct object types found in the input file"
  cp ""
  cp "\fg-v               \f0Verbose mode"



