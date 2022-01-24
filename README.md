# FileExchanger
## Installing FileExchanger as a Docker container

<ul>
  <li>Install Docker in your computer. "How to install Docker on a computer?" written on the <a href="https://www.docker.com/">official Docker website</a>.</br></li>
  <li>Pull the docker image <code>docker pull vodacode/fileexchanger</code></li>
  <li>
    On first start you should configure application.</br>
    Minimum requirements for the first run: </br>
    <code>docker run -d -p 8080:80 --name MyFileExchnager vodacode/fileexchanger -DB_HOST 127.0.0.1 -DB_PORT 1433 -DB_AUTH sa@password -FTP_HOST 127.0.0.1</code> - This command will launch a visa container named "<b>MyFileExchnager</b>" on port <b>8080</b>.</br>
    More details in the section "<a href="#configurations">Configurations</a>".
  </li>
  <li>
    For next runs you could use Docker Hub (GUI) or command line.</br>
    Run from the command line: <code>docker start MyFileExchnager</code>
  </li>
</ul>

## Configurations
<ul>
  <li>
    <code>-DB_HOST [<i>String</i>]</code> - Database server host.</br>
    Example: <code>-DB_HOST example.com</code> or <code>-DB_HOST 127.0.0.1</code>
  </li>
  <li>
    <code>-DB_PORT [<i>Number</i>]</code> - Database server port.</br>
    Example: <code>-DB_PORT 1433</code>
  </li>
  <li>
    <code>-DB_AUTH [<i>String</i>]</code> - Data for authorization on the database server.</br>
    Example: <code>-DB_AUTH username@password</code>
  </li>
  <li>
    <code>-DB_NAME [<i>String</i>]</code> - <i>(optional)</i> Database name.</br>
    Default: <b>FileExchanger_[<i>TICS_AT_INITIALIZATION</i>]</b></br>
    Example: <code>-DB_NAME FileExchanger</code>
   </li>
  <li>
    <code>-FTP_HOST [<i>String</i>]</code> - File server hostname.</br>
    Example: <code>-FTP_HOST example.com</code> or <code>-FTP_HOST 127.0.0.1</code>
  </li>
  <li>
    <code>-FTP_PORT [<i>Number</i>]</code> - <i>(optional)</i> File server port.</br>
    Default: <b>21</b></br>
    Example: <code>-FTP_PORT 21</code>
  </li>
  <li>
    <code>-FTP_AUTH [<i>String</i>]</code> - <i>(optional)</i> Data for authorization on the file server.</br>
    Default: <b>anon@anon</b></br>
    Example: <code>-FTP_AUTH username@password</code>
  </li>
  <li>
    <code>-FTP_SSL [<i>Boolean</i>]</code> - <i>(optional)</i> Enable ssl connection to ftp server.</br>
    Default: <b>0</b> - Disabled</br>
    Example: <code>-FTP_SSL 1</code>
  </li>
  <li>
    <code>-MaxSaveSize [<i>String</i>]</code> - <i>(optional)</i> The maximum file size that can be saved.</br>
    Default: <b>1.5 Gb</b></br>
    Format: <code>-MaxSaveSize [[<i>Number</i>] Gb/Mb/Kb]</code></br>
    Example: <code>-MaxSaveSize 2 Gb</code>
  </li>
  <li>
    <code>-MaxSaveTime [<i>String</i>]</code> - <i>(optional)</i> Maximum file storage time.</br>
    Default: <b>1d</b></br>
    Format: <code>-MaxSaveTime [[<i>Number</i>]d/h/m/s]..</code></br>
    Example: <code>-MaxSaveTime 1d 5h 40m 50s</code>
  </li>
  <li>
    <code>-MaxUploadCount [<i>Number</i>]</code> - <i>(optional)</i> Maximum number of file uploads.</br>
    Default: <b>2</b></br>
    Example: <code>-MaxUploadCount 3</code>
  </li>
</ul>
