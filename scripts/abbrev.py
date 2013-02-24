#!/usr/bin/env python
# -*- coding: UTF_8 -*-

#/////////////////////////////////////////////////////////////////////////////////
# Useful abbreviations to reduce typing work writing typical python mini-scripts.
# (c) Alexander Gessler, 2010
#
# HIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
# ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
# WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
# DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
# ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
# (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
# LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
# ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
# (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
# SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
# ///////////////////////////////////////////////////////////////////////////////////

# re module
import re
rec = re.compile

# os module
import os
opj = os.path.join
ope = os.path.exists
opse = os.path.splitext
ops = os.path.split
oprp = os.path.realpath
opap = os.path.abspath
opid = os.path.isdir
opif = os.path.isfile
opil = os.path.islink
opia = os.path.isabs
opgs = os.path.getsize
old = os.listdir

# itertools module
import itertools
itch = itertools.chain
itc = itertools.combinations
itp = itertools.permutations
itcp = itertools.product

# collections module
import collections
dd = collections.defaultdict

# vim: ai ts=4 sts=4 et sw=4

