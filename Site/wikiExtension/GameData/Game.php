<?php

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Game
 *
 * @author TKMAYN9
 */
class Game {

    public $name = null;
    public $title;
    public $versions = array();

    
    
    public function loadFromDb($name,$criteria) {
        $sql = 'select * from masgau_game_data.games where name = \''.$name.'\'';
        $result = mysql_query($sql);
        
        if($row = mysql_fetch_assoc($result)) {
            $this->name = $row['name'];
            $this->title = $row['title'];

            $sql = 'select * from masgau_game_data.game_versions where name = \'' . $name . '\'';
            if($criteria!=null) {
                $sql .= ' AND '.$criteria;
            }
            $result = mysql_query($sql);

            while ($sub_row = mysql_fetch_assoc($result)) {
                require_once 'GameVersion.php';
                $version = new GameVersion();
                $version->loadFromDb($sub_row['id']);
                
                $i = sizeof($this->versions);
                $this->versions[$i] = $version;
            }
        }        
        
        
    }   
    
    public function loadFromXml($node) {
        require_once 'GameVersion.php';
        global $wgOut;
        foreach ($node->attributes as $attribute) {
            switch ($attribute->name) {
                case 'name':
                    if($this->name==null)
                        $this->name = $node->attributes->getNamedItem('name')->value;
                    else if ($this->name != $attribute->value)
                        throw new Exception('GAME MISMATCH ' . $this->name . ' ' . $attribute->value);
                    break;
                //default:
                //throw new Exception($attribute->name.' not supported');
            }
        }
        $i = sizeof($this->versions);
        $version = new GameVersion();
        $this->versions[$i] = $version;
        $version->loadFromXml($node);
    }

    public function getTitle() {
        global $wgOut;
        
        $titles = array();
        foreach($this->versions as $version) {
            if($titles[$version->title]==null)
                $titles[$version->title] = 0;
            else
                $titles[$version->title]++;
        }
        $candidate = null;
        foreach(array_keys($titles) as $title) {
            if($titles[$title]>$titles[candidate]||$candidate==null) {
                $candidate = $title;
            }
        }
            
        return $candidate;
    }
    
    public function writeToDb($replace) {        
        global $wgOut;
        
        $dbr = wfGetDB(DB_SLAVE);
        $res = $dbr->select('masgau_game_data.games', 'count(*) as count', // $vars (columns of the table)
                        'name = \''.$this->name.'\'', // $conds
                        __METHOD__, // $fname = 'Database::select',
                        null
                    );

        
        
        if($res->fetchObject()->count==0) {
            $wgOut->addWikiText('* Writing game '.$this->getTitle().' to database ('.$this->name.')');
            $dbw = wfGetDB( DB_MASTER );
            $dbw->insert('masgau_game_data.games', 
                    array('name'=>$this->name,
                        'title'=>$this->getTitle(),), 
                    $fname = 'Database::insert', 
                    $options = array() );
        } else {
            $wgOut->addWikiText('* Game '.$this->getTitle().' ('.$this->name.') already exists in database');
        }
        
        foreach($this->versions as $version) {
            $version->writeToDb($replace);
        }
        
    }
}

?>
