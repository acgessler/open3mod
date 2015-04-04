#!/usr/bin/env python3
# -*- coding: UTF_8 -*-

# -----------------------------------------------------------------------------------
## Script to change the license/copyright headers of all source files at once.
## Matching via RE is supported. Footers also.
##
## Provides reasonable default settings for C#, C++, PHP, CG, HLSL, Python
##
## Almost all settings are configurable, files can be grouped, excluded or
## separated just by modifying this file. Take a look at the comments below.
## @author (c) 2009-2010, Alexander Gessler
# -----------------------------------------------------------------------------------

import array
from abbrev import *

minor = 0
major = 2
copyright = "(c) 2012-2015, Open3Mod Contributors"
project_name = "Open 3D Model Viewer (open3mod)"

# -----------------------------------------------------------------------------------
# Uniquely-named search patterns to group files automatically.
# This allows for arbitrary complex schemes. However, at the moment
# we're just looking for a particular file extension
# -----------------------------------------------------------------------------------
rep_match_anydir = r"([\w.]+[\\/])*"
regexes = dd(lambda: None, {
 ".py"  : rec(rep_match_anydir + r"\w+\.py$")
, ".cpp" : rec(rep_match_anydir + r"\w+\.cpp$")
, ".h"   : rec(rep_match_anydir + r"\w+\.h$")
, ".inl" : rec(rep_match_anydir + r"\w+\.inl$")
, ".hpp" : rec(rep_match_anydir + r"\w+\.hpp$")
, ".cg"  : rec(rep_match_anydir + r"\w+\.cg$")
, ".php" : rec(rep_match_anydir + r"\w+\.php$")
, ".cs"  : rec(rep_match_anydir + r"\w+\.cs$")
})

# -----------------------------------------------------------------------------------
# Map regexes to header templates. 
# -----------------------------------------------------------------------------------
extension_map = dd(lambda: "", {
 r".cs"   : opj("headers", "header_cs.txt")
, r".cpp"  : opj("headers", "header_cpp.txt")
, r".h"    : opj("headers", "header_h.txt")
, r".inl"  : opj("headers", "header_h.txt")
, r".hpp"  : opj("headers", "header_h.txt")
, r".cg"   : opj("headers", "header_cg.txt")
, r".php"  : opj("headers", "header_php.txt")
, r".cs"  : opj("headers", "header_cs.txt")
})

# -----------------------------------------------------------------------------------
# Map regexes to footer templates
# -----------------------------------------------------------------------------------
footer_extension_map = dd(lambda: "", {
 r".py"   : opj("headers", "footer_py.txt")
, r".cpp"  : opj("headers", "footer_cpp_h.txt")
, r".h"    : opj("headers", "footer_cpp_h.txt")
, r".inl"  : opj("headers", "footer_cpp_h.txt")
, r".hpp"  : opj("headers", "footer_cpp_h.txt")
, r".cg"   : opj("headers", "footer_cpp_h.txt")
, r".php"  : opj("headers", "footer_php.txt")
, r".cs"   : opj("headers", "footer_cs.txt")
})

# -----------------------------------------------------------------------------------
# For each language, specify the character sequence for line comments
# -----------------------------------------------------------------------------------
comment_indicators = dd(lambda: "//", {
 r".py"   : ("#",)
, r".php"  : ("//", "<?php", "?>")
})

# -----------------------------------------------------------------------------------
# For each language, specify start and end magic sequences for comments
# spanning over multiple lines. Can be an empty tuple if the language 
# does not provide multi-line comments.
# -----------------------------------------------------------------------------------
multiline_comment_indicators = dd(lambda: (('/*', '*/'),), {
 r".py"   : (('"""', '"""'),)
})

# -----------------------------------------------------------------------------------
# Maximum number of empty, non-comment, lines to be tolerated until the 
# script assumes that the end of the previous license header has been
# passed
# -----------------------------------------------------------------------------------
maxemptylines = dd(lambda: 0, {
 r".py"   : 1
, r".php"  : 1
})

OPTION_APPEND_FINAL_LINEFEED = 0x1
# -----------------------------------------------------------------------------------
# Mixed bitflags denoting special features to be enabled or disable
# for files with a particular file extensions. See the list above.
# -----------------------------------------------------------------------------------
mixed_options = dd(lambda: 0, {
  r".cpp"  : OPTION_APPEND_FINAL_LINEFEED
, r".h"    : OPTION_APPEND_FINAL_LINEFEED
, r".cg"   : OPTION_APPEND_FINAL_LINEFEED
})

# -----------------------------------------------------------------------------------
# Directories to be processed. Processing is not recursive, every single directory
# needs to be specified. For each files in the input directories, the script 
# searches for a positive regex match via the 'regexes' table. Note that the
# regexes check is performed on the full os path of a file, so additional per-path
# filtering can be done easily.
# -----------------------------------------------------------------------------------
directories = [
  opj("..", "open3mod")
]

# -----------------------------------------------------------------------------------
# Files to be excluded - can be both a file name or a fully qualified
# patch to exclude a specific file. This is a much simpler way to exclude files
# from being processed than adding them to the 'regexes' table.
# -----------------------------------------------------------------------------------
exclude = [
 ],

# -----------------------------------------------------------------------------------
# Text replacements to be performed. Currently, these values are gathered from the
# various getXXXX mini scripts scattered throughout this directory. These files
# are generated by the genversioninfo.py script and represent the latest 
# configuration the application was built for.
# -----------------------------------------------------------------------------------
replacements = {
 r"<copyright>"     : copyright
, r"<project-name>"  : project_name
, r"<major>"         : str(major)
, r"<minor>"         : str(minor)
}

# -----------------------------------------------------------------------------------
# Output encoding for all source code files
# -----------------------------------------------------------------------------------
input_encoding = dd(lambda: "cp1252", {
 r".cs"   : "utf-8-sig"
})

# -----------------------------------------------------------------------------------
# Output encoding for all source code files
# -----------------------------------------------------------------------------------
output_encoding = dd(lambda: r"cp1252", {
 r".cs"   : "utf-8"
})

# -----------------------------------------------------------------------------------
# Output line feed sequence. Either '\r\n' (Windows style), '\n' (Unix style),
# '\r' (OS2 style). Project default is currently Windows.
# -----------------------------------------------------------------------------------
output_linefeed = dd(lambda: "\r\n", {
})

# -----------------------------------------------------------------------------------
cached_files = {}
def load_file_into_cache(min):
    if min not in cached_files:
        m = open(min, "rt", errors="ignore").read()
        for r0, r1 in replacements.items():
            m = m.replace(r0, r1)
            
        cached_files[min] = m
        
    else:
        m = cached_files[min]
    return m

# -----------------------------------------------------------------------------------
def find_match(t, comment):
    for tc in comment:
        if t[:len(tc)] == tc:
            return True
    return False

# -----------------------------------------------------------------------------------
def find_substr_match(t, comment, sub):
    for tc in comment:
        if t.find(tc[sub]) != -1:
            return True
        
    return False

# -----------------------------------------------------------------------------------
def run():
    stats = dd(lambda: 0)
    for p in directories:
        print("DIR ENTER: " + p)

        def safeold(p):
            try:
                for t in old(p):
                    yield t
            except:
                print("   DIRECTORY IS NOT ACCESSIBLE: " + p)
        
        for t in safeold(p):
            ful = opj(p, t)
            
            if opid(ful):
                continue
                
            print("   FILE: " + ful)
            if p in exclude or ful in exclude:
                print("   SKIPPING FILE - MATCH IN EXCLUDE LIST")
                continue

            ext = None
            for key, value in regexes.items():
                if re.match(value, ful):
                    ext = key
                    break

            if not ext:
                print("   SKIPPING FILE - NO REGEX PATTERN MATCHING")
                continue

            # read all lines of the input file
            fin = open(ful, "rt", errors="ignore", encoding=input_encoding[ext])
            if not fin:
                print("   FAILURE OPENING FILE")
                continue
          
            fil = fin.readlines()
            if 0 == len(fil):
                continue

             # cache comment sequences as tuple
            comment = comment_indicators[ext]
            if isinstance(comment, str):
                comment = (comment,)

            ml_comment = multiline_comment_indicators[ext]
            if isinstance(ml_comment, tuple) and isinstance(ml_comment[0], str):
                ml_comment = (ml_comment,)

            min = extension_map[ext]
            if len(min):
                m = load_file_into_cache(min)    
                m = m.replace("<name>", t)

                # Remove the old header: remove everything to the
                # first non-comment line, skipping maximally one
                # group of empty lines. This should work fine.
                #
                # FIXME: why not simply regex for the old header?
                # At least optionally this would be a nice feature,
                # especially as many headers use keywords at their 
                # start and end respectively.
                was_empty = outp = incomment = False
                gcount = 0
                arr = m + "\n"		    
                
                for lins in fil:
                    if outp:
                        arr = arr + lins
                        continue
                        
                    if not incomment:
                        if find_substr_match(lins, ml_comment, 0):
                            incomment = True

                    if incomment:
                        if find_substr_match(t, ml_comment, 1):
                            incomment = False

                        if incomment:
                            continue
                    
                    if len(lins.strip()) <= 1:
                        if not was_empty:
                            gcount = 1 + gcount
                        
                            # for python, we're tolerating one group of some space lines, but
                            # the second is treated as end of the header section
                            if gcount > maxemptylines[ext]:
                                outp = True                       
                                            
                            was_empty = True
                        
                    else: 
                        if not find_match(lins, comment):
                            # non-comment line which is also not empty -> end of header
                            outp = True
                            arr = arr + lins
                        
                        else:
                            was_empty = False
            else:
                arr = ''.join(fil)

            min = footer_extension_map[ext]    
            if len(min):
                m = load_file_into_cache(min)    
                m = m.replace("<name>", t)

                # Split into lines - again. Not efficient, but comfortable.
                rsplit = list(enumerate(arr.rsplit("\n")))
                arr = ""
                incomment = False
                for i, t in reversed(rsplit):
                    if not len(t) or ((not incomment) and find_match(t, comment)):
                        continue
                    
                    if not incomment:
                        if find_substr_match(t, ml_comment, 1):
                            incomment = True

                    if incomment:
                        if find_substr_match(t, ml_comment, 0):
                            incomment = False

                        continue
                    #print(t)
                    # non-comment line
                    for ii, tt in rsplit: 
                        arr += tt + "\n"

                        if ii == i:
                            break

                    arr += "\n"
                    arr += m

                    # append the line feed required by most programming languages
                    if mixed_options[ext] & OPTION_APPEND_FINAL_LINEFEED:
                        arr += "\n"

                    break
                else:
                    # not a single non-comment line .. keep the whole file
                    arr = ''.join(map(lambda x: x[1] + '\n', rsplit))
                             
            # fix line feeds
            arr.replace('\n', output_linefeed[ext])

            fin.close()
            fin = open(ful, "w", encoding=output_encoding[ext], errors="ignore")
            fin.write(arr)
              
            fin.close()
            print("   SUCCESS")
            stats[ext] = stats[ext] + 1
                
        print("DIR LEAVE: " + p)

    print("")
    # finally, print out file statistics
    for key, value in stats.items():
        print("Regex: {key}\tMatches: {value}".format(**locals()))

# -----------------------------------------------------------------------------------
if __name__ == "__main__":
    #global directories
    import sys
    run()
                
# vim: ai ts=4 sts=4 et sw=4

