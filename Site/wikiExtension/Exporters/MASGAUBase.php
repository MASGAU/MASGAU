<?php

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of MASGAUBase
 *
 * @author TKMAYN9
 */
class MASGAUBase {

    public $xml;
    protected $root;
    protected $major, $minor;
    public static $date_format = 'Y-m-d\TH:i:s';

    //public function exportGameVersion();
    //put your code here

    public static function formatDate($string) {
        return date_format(new DateTime($string), self::$date_format);
    }

    protected function __construct($major, $minor, $file) {
        $this->major = $major;
        $this->minor = $minor;

        if ($file != null) {

            $this->xml = new DOMDocument();
            $this->xml->encoding = 'UTF-8';

            $this->xml->formatOutput = true;
            $this->root = $this->xml->appendChild($this->xml->createElement("games"));
            $this->root->appendChild($this->xml->createAttribute("xmlns:xsi"))->
                    appendChild($this->xml->createTextNode("http://www.w3.org/2001/XMLSchema-instance"));
            $this->root->appendChild($this->xml->createAttribute("xsi:noNamespaceSchemaLocation"))->
                    appendChild($this->xml->createTextNode("games.xsd"));
            $this->root->appendChild($this->xml->createAttribute("majorVersion"))->
                    appendChild($this->xml->createTextNode($major));
            $this->root->appendChild($this->xml->createAttribute("minorVersion"))->
                    appendChild($this->xml->createTextNode($minor));


            $sql = 'select * from masgau_game_data.xml_files where name = \'' . $file . '\'';
            $result = mysql_query($sql);
            if ($row = mysql_fetch_assoc($result)) {
                $this->root->appendChild($this->xml->createAttribute("date"))->
                        appendChild($this->xml->createTextNode(
                                        self::formatDate($row['last_updated'])));
                if ($row['comment'] != null)
                    $this->root->appendChild($this->xml->createComment($row['comment']));
            } else {
                throw new Exception('Could not find specified file in database: ' . $file);
            }
        }
    }

    function export($games) {
        foreach ($games->games as $game) {
            foreach ($game->versions as $version) {
                // Write root game tag
                $this->root->appendChild($this->exportGameVersion($game, $version));
            }
        }
        if (file_exists($_SERVER["DOCUMENT_ROOT"] . '/wiki/extensions/MASGAU/Schemas/MASGAU-' . $this->major . '.' . $this->minor . '.xsd')) {
            $schema = $_SERVER["DOCUMENT_ROOT"] . '/wiki/extensions/MASGAU/Schemas/MASGAU-' . $this->major . '.' . $this->minor . '.xsd';
        } else {
            $schema = $_SERVER["DOCUMENT_ROOT"] . '/extensions/MASGAU/Schemas/MASGAU-' . $this->major . '.' . $this->minor . '.xsd';
        }

        if (!$this->xml->schemaValidate($schema))
            throw new Exception("XML DID NOT PASS VALIDATION: " . $schema);



        return $this->xml->saveXML();
    }

    protected static function cleanUp($string) {
        //$string = str_replace('"', '&quot;', $string);
        $string = str_replace('&', '&amp;', $string);
        //$string = str_replace('', '&apos;', $string);
        //$string = str_replace('<', '&lt;', $string);
        //$string = str_replace('>', '&gt;', $string);
        
        return $string;
    }

    protected function createTextNode($text) {
        return $this->xml->createTextNode(self::cleanUp($text));
    }

    protected function createElement($name, $content = null) {
        if ($content == null) {
            return $this->xml->createElement($name);
        } else {
            return $this->xml->createElement($name, self::cleanUp($content));
        }
    }

}

?>
