# -*- coding: utf-8 -*-
import re

# constants
sep = ['《' , '》' , '【' , '】' , '～' , '「' , '」' , '/' , '　', '『', '』']
output_sep = '-'

#main block
org_title = _title_converter.Title
org_desc= _title_converter.ParseDescription(_title_converter.Description)
tokens = _title_converter.SplitToken(org_title, tuple(sep) )

title = ''
band_name = ''

for s in tokens:
	token = s.strip()
	if _title_converter.IsBandName(token):
		band_name = token
	elif _title_converter.IsIgonoreToken(token):
		continue
	else:
		title += token + output_sep

if title[-1:] == output_sep:
	title = title[:-1]

title_from_desc = _title_converter.GetTitle(org_desc)
if title_from_desc:
    title = title_from_desc

_result = _title_converter.ParseOutputPartten(band_name,'',title)

_logger.WriteLine("saved " + _result)
