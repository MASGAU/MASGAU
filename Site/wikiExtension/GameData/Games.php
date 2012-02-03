<?php

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/* 
 * Description of Games
 *
 * @author TKMAYN9
 */
class Games {
    private $document;
    public $games = array();

    public function loadFromXml($xml_file) {
        global $wgOut;
        //$wgOut->addWikiText($xml_file);

        $this->document = new DOMDocument();
        $this->document->load($xml_file);
        
        if(!$this->document->schemaValidate('extensions/MASGAU/Schemas/MASGAU-1.2.xsd'))
            throw new Exception("VALIDATION FAILED!!!!");
        
        $nodes = $this->document->getElementsByTagName('games')->item(0);

        require_once('Game.php');
        foreach ($nodes->childNodes as $node) {
            if ($node->localName == 'game') {
                $name = $node->attributes->getNamedItem('name')->value;
                if ($this->games[$name] != null) {
                    $game = $this->games[$name];
                } else
                    $game = new Game();
                $game->loadFromXml($node);

                $this->games[$name] = $game;
            }
        }
    }

    public function loadFromDb($file) {
        if($file!=null) {
            $sql = 'select * from masgau_game_data.xml_files where name = \''.$file.'\'';
            $result = mysql_query($sql);
            if($row = mysql_fetch_assoc($result)) {
                $criteria = $row['game_criteria'];
                $version_criteria = $row['version_criteria'];
            }
        }
        if($criteria!=null)
            $sql = 'select * from masgau_game_data.games WHERE '.$criteria.' order by name asc';
        else
            $sql = 'select * from masgau_game_data.games order by name asc';
        $result = mysql_query($sql);

        require_once('Game.php');

        while ($row = mysql_fetch_assoc($result)) {
            $game = new Game();
            $game->loadFromDb($row['name'],$version_criteria);
            $this->games[$game->name] = $game;
        }        
    }
    
    public function writeToDb($replace) {
        global $wgOut;
        $wgOut->addWikiText('Writing games to database');
        foreach ($this->games as $game) {
            $game->writeToDb($replace);
        }
    }

}

?>
