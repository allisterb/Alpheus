# Alpheus: Cross-platform configuration file parser
Get the latest release from the [releases](https://github.com/allisterb/Alpheus/releases) page or through [NuGet](https://www.nuget.org/packages/Alpheus.Core/)

![Screenshot](https://1qirkq.dm2301.livefilestore.com/y4mBoMY8wR3dfFOclfZKWnIZtrYC68PNYM3adTZCN9WUtZEzcnZhPAqvXseSkBsEnuB3vAvZN45fDx7MbNoAuqhFEDTu73qwqH2OZxtp-C-j7XYGr1MhjXdLCfGGDhipzTIwmgX7P3rB1huY-u8hl1JMQxWjf4XJzUyga2eN8b9-0cSO6YYufKhzQ6wrgKvxXTEsx2EDQ8id8S_sZ8D1BuDog?width=1121&height=799&cropmode=none)

## About
Alpheus is a parser and query tool for system and server configuration files. Alpheus parses and transforms configuration files into an XML representation which can then be queried using XPATH. E.g. from the following fragment from a MySQL `my.cnf` configuration file:

````
#
# * InnoDB
#
# InnoDB is enabled by default with a 10MB datafile in /var/lib/mysql/.
# Read the manual for more InnoDB related options. There are many!
innodb_additional_mem_pool_size=256M
innodb_buffer_pool_size=20GB
# number of CPU cores dedicated to the MySQL InnoDB backend 
innodb_buffer_pool_instances = 4
innodb_log_buffer_size=256M
innodb_log_file_size=1G
bulk_insert_buffer_size=256M
innodb_flush_log_at_trx_commit=2
innodb_flush_method=O_DIRECT
innodb_doublewrite = 0
innodb_file_per_table = 1
innodb_file_format = barracuda

[mysqldump]
quick
quote-names
max_allowed_packet	= 16M

[mysql]
#no-auto-rehash	# faster start of mysql but no tab completition

[isamchk]
key_buffer		= 16M

!includedir /etc/mysql/conf.d/
````

Alpheus transforms the sections and directives in the configuration file into the following XML:
````                                                                
    ...
    <innodb_log_buffer_size Position="4953" Column="1" Line="148" Length="22" File="my.cnf">         
      <Value Position="4976" Column="24" Line="148" Length="4">256M</Value>                          
    </innodb_log_buffer_size>                                                                        
    <innodb_log_file_size Position="4982" Column="1" Line="149" Length="20" File="my.cnf">           
      <Value Position="5003" Column="22" Line="149" Length="2">1G</Value>                            
    </innodb_log_file_size>                                                                          
    <bulk_insert_buffer_size Position="5007" Column="1" Line="150" Length="23" File="my.cnf">        
      <Value Position="5031" Column="25" Line="150" Length="4">256M</Value>                          
    </bulk_insert_buffer_size>                                                                       
    <innodb_flush_log_at_trx_commit Position="5037" Column="1" Line="151" Length="30" File="my.cnf"> 
      <Value Position="5068" Column="32" Line="151" Length="1">2</Value>                             
    </innodb_flush_log_at_trx_commit>                                                                
    <innodb_flush_method Position="5071" Column="1" Line="152" Length="19" File="my.cnf">            
      <Value Position="5091" Column="21" Line="152" Length="8">O_DIRECT</Value>                      
    </innodb_flush_method>                                                                           
    <innodb_doublewrite Position="5101" Column="1" Line="153" Length="18" File="my.cnf">             
      <Value Position="5122" Column="22" Line="153" Length="1">0</Value>                             
    </innodb_doublewrite>                                                                            
    <innodb_file_per_table Position="5125" Column="1" Line="154" Length="21" File="my.cnf">          
      <Value Position="5149" Column="25" Line="154" Length="1">1</Value>                             
    </innodb_file_per_table>                                                                         
    <innodb_file_format Position="5152" Column="1" Line="155" Length="18" File="my.cnf">             
      <Value Position="5173" Column="22" Line="155" Length="9">barracuda</Value>                     
    </innodb_file_format>                                                                            
  </mysqld>                                                                                          
  <mysqldump File="my.cnf">                                                                          
    <quick Position="5494" Column="1" Line="172" Length="5" File="my.cnf">                           
      <Value Position="5494" Column="1" Line="172" Length="4">true</Value>                           
    </quick>                                                                                         
    <quote-names Position="5501" Column="1" Line="173" Length="11" File="my.cnf">                    
      <Value Position="5501" Column="1" Line="173" Length="4">true</Value>                           
    </quote-names>                                                                                   
    <max_allowed_packet Position="5514" Column="1" Line="174" Length="18" File="my.cnf">             
      <Value Position="5535" Column="22" Line="174" Length="3">16M</Value>                           
    </max_allowed_packet>                                                                            
  </mysqldump> 
  <mysql File="my.cnf" />
  <isamchk File="my.cnf">
    <key_buffer Position="5629" Column="1" Line="180" Length="10" File="my.cnf">
      <Value Position="5643" Column="15" Line="180" Length="3">16M</Value>
    </key_buffer>
    <includedir Position="5800" Column="2" Line="186" Length="10" File="my.cnf">
      <Value Position="5811" Column="13" Line="186" Length="18">/etc/mysql/conf.d/</Value>
    </includedir>
  </isamchk>
</MySQL>
  ````
You can then query the XML representation using the XPATH query language e.g. the following screenshot shows a query of the `Port` directive in the `[mysqld]` section of the `my.cnf` file:

![Query screenshot](https://1qik4g.dm2301.livefilestore.com/y4mCaV-1xfjcayXYIl7SrtBrrrJr6vmdO366CkHgXtNdi6cMdQWiHIrqiZ0Gw9KT1JbhPvLC1b-GFkWmwXWFSzWf4EvcHK5iubR-JqSOMa-RA1n1FRozOxEjV0BvszNNSXHUk55KqNCKVRem4_I7cnQ8quFHUMbGwpdmTvlNzogrSsB6R9VZxWItPxCZxYoteUfc9ki2YoiPR04b42YaiEFsA?width=1106&height=796&cropmode=none)

## Supported formats
Alpheus can parse and query configuration files for the following servers and applications:
* OpenSSH (sshd_config)
* MySQL (my.cnf)
* PostgreSQL (postgresql.conf)
* Nginx (nginx.conf)
* Apache Httpd (http
* Docker (Dockerfile)
* .NET and ASP.NET App.config and Web.Config files

Alpheus is similar in goals to the [Augeas](http://augeas.net/) project but with quite different execution:

* Augeus is written for Linux with only [nascent Windows support](https://github.com/hercules-team/augeas/issues/476) that requires a compatibilty layer like Cygwin. Alpheus runs on any platform with .NET support: .NET Framework, Mono, or .NET Core. 

* Augeas is written in C and uses the [Boomerang](https://alliance.seas.upenn.edu/~harmony/) language which is a subset of ML for writing lenses. Alpheus is written in C# and uses the [Sprache](https://github.com/sprache/Sprache) monadic parser combinator library. Parser combinators are a good match for OOP languages with functional bits like C#. Sprache and C# allow you to use functional ideas while incrementally building and testing parsers and reusing existing grammar pieces, e.g. the following code is a part of the MySQL grammar:
````
            public static Parser<AString> KeyName
            {
                get
                {
                    return AStringFrom(AlphaNumericIdentifierChar.Or(Underscore).Or(Dash));
                }
            }

            public static Parser<AString> KeyValue
            {
                get
                {
                    return AnyCharExcept("'\"\r\n");
                }
              
            }

            public static Parser<AString> QuotedKeyValue
            {
                get
                {
                    return DoubleQuoted(Optional(KeyValue)).Or(SingleQuoted(Optional(KeyValue)));
                }

            }

            public static Parser<AString> SectionName
            {
                get
                {
                    return
                        from w1 in OptionalMixedWhiteSpace
                        from ob in OpenSquareBracket
                        from sn in SectionNameAString
                        from cb in ClosedSquareBracket
                        select sn;
                }
            }
````
Functions as first-class objects together with LINQ expressions are used to construct the parser grammar in C# while reusing and combining existing parser bits.

* Augeas reads local file-system files only. Alpheus abstracts the I/O operations required for reading files into an interface and can read files from any class that implements the interface. For instance the DevAudit project implements I/O environments for SSH, GitHub, Docker containers et.al and uses Alpheus to directly parse and query configuration files from remote environments.

* Alpheus understands the semantics of configuration file in addition to the syntax. For instance Alpheus can recognize MySQL `include` and `includedir` directives and inserts the parsed included files into the XML representation. E.g. from the follwing MySQL configuration:
````
[mysqlhotcopy]
interactive-timeout


!includedir mysql.conf.d
````
if the `mysql.conf.d` directory has a file called `my.2.cnf` then the following XML will be produced:
````
<mysqlhotcopy File="my-large.cnf">                                                                
  <interactive-timeout Position="2591" Column="1" Line="88" Length="19" File="my-large.cnf">      
    <Value Position="2591" Column="1" Line="88" Length="4">true</Value>                           
  </interactive-timeout>                                                                          
  <includedir Position="2617" Column="2" Line="91" Length="10" File="my-large.cnf">               
    <Value Position="2628" Column="13" Line="91" Length="12">mysql.conf.d</Value>                 
  </includedir>                                                                                   
</mysqlhotcopy>                                                                                   
<client File="my.2.cnf">                                                                          
  <port Position="737" Column="1" Line="22" Length="4" File="my.2.cnf">                           
    <Value Position="745" Column="9" Line="22" Length="4">3306</Value>                            
  </port>                                                                                         
  <socket Position="751" Column="1" Line="23" Length="6" File="my.2.cnf">                         
    <Value Position="761" Column="11" Line="23" Length="27">/var/run/mysqld/mysqld.sock</Value>   
  </socket>                                                                                       
</client>                                                                                         
<mysqld_safe File="my.2.cnf">                                                                     
  <socket Position="807" Column="1" Line="26" Length="6" File="my.2.cnf">                         
    <Value Position="817" Column="11" Line="26" Length="27">/var/run/mysqld/mysqld.sock</Value>   
  </socket>                                                                                       
  <nice Position="846" Column="1" Line="27" Length="4" File="my.2.cnf">                           
    <Value Position="854" Column="9" Line="27" Length="1">0</Value>                               
  </nice>                                                                                         
</mysqld_safe>                                                                                    
````

## Usage
### Command Line Interface
Download and unzip the release archive. Type `al -v` and `al -h` to see the version informtion and help with using the CLI.

### Library
Install the [NuGet](https://www.nuget.org/packages/Alpheus.Core/) into your application. You can read and parse a file like this:
`MySQL mysql = new MySQL(this.ConfigurationFile, this.AlpheusEnvironment);` See the Alpheus CLI source code and tests for examples on how to use the library.

## Building
Clone the Github repository on to your computer. You can build for .NET Framework with `build-netfx` on Windows or `./build-netcfx.sh` on Linux. You can also build for .NET Core with `build-netcore` on Windows or `./build-netcore.sh` on Linux.

