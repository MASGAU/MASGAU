<?php
function curPageURL() {
 $pageURL = 'http';
 $pageURL .= "://";
 if ($_SERVER["SERVER_PORT"] != "80") {
  $pageURL .= $_SERVER["SERVER_NAME"].":".$_SERVER["SERVER_PORT"].$_SERVER["REQUEST_URI"];
 } else {
  $pageURL .= $_SERVER["SERVER_NAME"].$_SERVER["REQUEST_URI"];
 }
 return $pageURL;
}

if (isset($_GET['version'])) {
    ini_set('default_charset', 'UTF-8');
    header("Content-Type:text/xml; charset=UTF-8'");
    define('MASGAU', 'whut');


    require_once '../DBSettings.php';
    $link = mysql_connect($wgDBserver, $wgDBuser, $wgDBpassword);
    mysql_set_charset('utf8',$link);


    if (isset($_GET['file'])) {
        //iconv_set_encoding("internal_encoding", "UTF-8");
        //iconv_set_encoding("output_encoding", "UTF-8");
        require_once '../extensions/MASGAU/Exporters/MASGAU-' . $_GET['version'] . '.php';

        require_once '../extensions/MASGAU/GameData/Games.php';
        $games = new Games();

        $games->loadFromDb($_GET['file']);

        $exporter = new Exporter($_GET['file']);
        echo $exporter->export($games);
    } else {
        $xml = new DOMDocument();
        $xml->encoding = 'utf-8';
        $xml->formatOutput = true;
        $root = $xml->appendChild($xml->createElement("files"));

        require_once '../extensions/MASGAU/Exporters/MASGAUBase.php';

        $sql = 'select * from masgau_game_data.program_versions order by major desc, minor desc, revision desc';
        $result = mysql_query($sql);
        if ($row = mysql_fetch_assoc($result)) {
            $file = $root->appendChild($xml->createElement("program"));
            $file->appendChild($xml->createAttribute("majorVersion"))->
                    appendChild($xml->createTextNode($row['major']));
            $file->appendChild($xml->createAttribute("minorVersion"))->
                    appendChild($xml->createTextNode($row['minor']));
            $file->appendChild($xml->createAttribute("revision"))->
                    appendChild($xml->createTextNode($row['revision']));
            $file->appendChild($xml->createAttribute("url"))->
                    appendChild($xml->createTextNode($row['url']));
            $file->appendChild($xml->createAttribute("date"))->
                    appendChild($xml->createTextNode(MASGAUBase::formatDate($row['release_date'])));
        }

        $sql = 'select * from masgau_game_data.xml_files order by name asc';
        $result = mysql_query($sql);
        while ($row = mysql_fetch_assoc($result)) {
            $file = $root->appendChild($xml->createElement("file"));
            $file->appendChild($xml->createAttribute("name"))->
                    appendChild($xml->createTextNode($row['name']));
            $file->appendChild($xml->createAttribute("last_updated"))->
                    appendChild($xml->createTextNode(
                                    MASGAUBase::formatDate($row['last_updated'])));
            $file->appendChild($xml->createAttribute("url"))->
                    appendChild($xml->createTextNode(curPageURL().'&file='.$row['name']));
        }

        $output = $xml->saveXML();
        echo $output;
    }
} else {
    echo "VERSION NOT PROVIDED";
}
?>