# -*- coding: utf-8 -*-
import re

org_title = _title_converter.Title
org_desc= _title_converter.ParseDescription(_title_converter.Description)
sep = ['《' , '》' , '【' , '】' , '～' , '「' , '」' , '/' , ' ' , '　']
tokens = _title_converter.SplitToken(org_title, tuple(sep) )

title = ''
band_name = ''

for s in tokens:
	if _title_converter.IsBandName(s):
		band_name = s
	elif s in ['東方Vocal', 'FullMV', 'MMD']:
		continue
	else:
		title += s + '-'

if title[-1:] == '-':
	title = title[:-1]

m = re.match("曲名：(.+)",org_desc)
if m:
    title = m.group(1)

_result = _title_converter.ParseOutputPartten(band_name,'',title)

_logger.WriteLine("saved " + _result)
