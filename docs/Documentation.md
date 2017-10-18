# How Sitebuilder works

Sitebuilder is a console executable. It generates your pages in three steps :

# it appends all the parts files (like header, menu, content, footer, etc.)
# it suppress all the lines marked to be deleted (with <!--#delete-->)
# it replaces tags by there current values (tags often begins with # but you can choose the tag you want)
This little exe is smart because if all your utf-8 files begin with a Byte Order Mark it will not interpolate the BOM when appening all this stuff.
**Important** : Because of the ability of marking lines to be deleted, each file parts can be valid HTML and can be edited in Visual Studio without syntax error (i.e. all parts can have a head and a body that will be deleted after concatenation. You can insert special tags that will represent the current title of the page after substitution or that will represent the id of the page to be processed by JavaScript.

For example, if you have in your HTML file parts tags like 
* {"<title>#pagetitle</title>"}
* <a href="#backhref">Previous page</a>
* <script type="text/javascript">var pageID = "#pageid";</script>
and if you pass as command line args :
* #pagetitle=My page title
* #backhref=page11.html
* #pageid=12
all the occurrences of the left tags in the result file will be replaced by the right value. The result will be :
* {"<title>My page title</title>"}
* <a href="page11.html">Previous page</a>
* <script type="text/javascript">var pageID = "12";</script>
If you have in one of your part file a line like :
* </div><!--#delete-->
then this line won't be in the result file.

You can append as much files as you want and you can substitute as much tags as you want.

# Syntax
{{
sitebuilder <txtfile1> <txtfile2> [<txtfileN>...](_txtfileN_...) <resfile> [#var=newval](#var=newval)*");
sitebuilder /utf16 <file1> <file2> [<fileN>...](_fileN_...) <resfile> [#var=newval](#var=newval)*");
sitebuilder /utf32 <file1> <file2> [<fileN>...](_fileN_...) <resfile> [#var=newval](#var=newval)*"); }}
* The firt parameters are the files to append.
* The last provided file name is considered to be the result file to generate.
* the #tag=new value parameters are optionals.
* Use " if you have newval with spaces. For example "#title=My title with spaces".
* Sitebuilder can manage files with utf-8 BOM but also utf-16 or utf-32 little endian text files without interpoling the BOM. /utf8 is the default option value.
* All files to append and process must be in the same encoding (utf-8 or utf-16 or utf-32)
* The tag to mark a line to be suppressed in the files is the html comment <!--#delete-->

# Sample

If you type
sitebuilder.exe sources\header.htm sources\content12.htm sources\footer.htm page12.html "#title=My title" #menuid=menu12 #backhref=page11.html

you will produce in the current dir page12.html file by concatening header.htm, content12.htm and footer.htm files. All lines containing <!--#delete--> comment will be deleted. Then, all occurrences of #title, #menuid and #backhref tags in the result file are replaced by the provided values.

# Sample file parts
## Common header
{{
<!DOCTYPE html>
<html>
<head>
    <title>#pagetitle</title>
</head>

<body>
    <div>
        <!-- Header -->
        <script type="text/javascript">var mi = "#pageid";</script>
        <a href="#backhref">Common link</a>
        <!-- Content --> <!--#delete-->
        <!-- Footer  --> <!--#delete-->
    </div>               <!--#delete-->
</body>                  <!--#delete-->
</html>                  <!--#delete-->
}}
## Content
{{
<!DOCTYPE html>      <!--#delete-->
<html>               <!--#delete-->
<head>               <!--#delete-->
    <title></title>  <!--#delete-->
</head>              <!--#delete-->
<body>               <!--#delete-->
    <div class="page-region">
    <!-- Content -->
    </div>
</body>              <!--#delete-->
</html>              <!--#delete-->
}}
## Common footer
{{
<!DOCTYPE html>      <!--#delete-->
<html>               <!--#delete-->
<head>               <!--#delete-->
    <title></title>  <!--#delete-->
</head>              <!--#delete-->
<body>               <!--#delete-->
        <div>        <!--#delete-->
        <-- Footer -->
        </div>
</body>  
</html>  
}}
# Hint

To build a full site, you will have to put all of your common parts and all of your pages content in a "sources" subfolder.
Then you will edit a build.cmd script that contains all the generation commands for all your files.
for example : 

{{
sitebuilder.exe sources\header.htm sources\page1.content.htm sources\footer.htm page1.html #menuid=1 #backhref=index.html
sitebuilder.exe sources\header.htm sources\page2.content.htm sources\footer.htm page2.html #menuid=2 #backhref=page1.html
sitebuilder.exe sources\header.htm sources\page3.content.htm sources\footer.htm page3.html #menuid=3 #backhref=page2.html
}}
Then, each time you change the common parts of the files or each time you add a new page in your site, launch the build.cmd script to regenerate all the pages of your site in the current folder.


