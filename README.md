# Alpheus: Cross-platform configuration file parser

![Screenshot](https://1qirkq.dm2301.livefilestore.com/y4mBoMY8wR3dfFOclfZKWnIZtrYC68PNYM3adTZCN9WUtZEzcnZhPAqvXseSkBsEnuB3vAvZN45fDx7MbNoAuqhFEDTu73qwqH2OZxtp-C-j7XYGr1MhjXdLCfGGDhipzTIwmgX7P3rB1huY-u8hl1JMQxWjf4XJzUyga2eN8b9-0cSO6YYufKhzQ6wrgKvxXTEsx2EDQ8id8S_sZ8D1BuDog?width=1150&height=650&cropmode=none)

## About
Alpheus is a parser and query tool for system and server configuration files. Alpheus parses and transforms configuration files into an XML representation which can then be queried using XPATH. E.g. the following fragment from a MySQL `my.cnf` configuration file:

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
* OpenSSH sshd_config
* MySQL
* PostgreSQL
* Nginx
* Apache Httpd
* .NET and ASP.NET App.config and Web.Config files

Alpheus is similar in goals to the [Augeas](http://augeas.net/) project but with a quite different execution:

* Augeus is written for Linux with only [nascent Windows support](https://github.com/hercules-team/augeas/issues/476) that requires a compatibilty layer like Cygwin. Alpheus runs on any platform with .NET support, either .NET Framework (Windows and Mono on Linux) or .NET Core. 

* Augeas is written in C and uses the [Boomerang](https://alliance.seas.upenn.edu/~harmony/) language which is a subset of ML for writing lenses. Alpheus is written in C# and uses the [Sprache](https://github.com/sprache/Sprache) monadic parser combinator library. Parser combinators like Sprace are a good match for OOP languages like C# and they allow you to incrementally build and test parsers while reusing existing pieces

Augeas reads files on local filesystem files only. Alpheus abstracts the I/O operations required into an interface and can read files from any nre
. For instance the DevAudit project implements I/O environments for

Alpheus understands the semantics of configuration file in addition to the syntax. For instance Alpheus can recognize directi
