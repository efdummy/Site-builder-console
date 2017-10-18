# Do you need Sitebuilder ?
If you want to 
* generate html utf-8 pages for a web site where all the pages contains common html parts
* do it with a very SIMPLE tool.
you need Sitebuilder !

# How Sitebuilder works
Sitebuilder is a console executable. It generates your pages in three steps :
# it appends all the parts files (like header, menu, content, footer, etc.)
# it suppress all the lines marked to be deleted (with <!--#delete-->)
# it replaces tags by there current values (tags often begins with # but you can choose the tag you want)
This little exe is smart because if all your utf-8 files begin with a Byte Order Mark it will not interpolate the BOM when appening all this stuff.

**Important** : Because of the ability of marking lines to be deleted, each file parts can be valid HTML and can be edited in Visual Studio (or in any html editor) without syntax error (i.e. all parts can have a head and a body that will be deleted after concatenation. You can insert special tag that will represent the current title of the page after substitution or that will represent the id of the page to be processed by JavaScript.