<?php

require_once 'Location.php';
class GameLocation extends Location {
    //put your code here
    
    public $name;
    public $platform;
    public $region;
    
    public function loadFromDb($id) {
        parent::loadFromDb($id);
        $sql = 'select * from masgau_game_data.game_parents where id = '.$id.'';
        $result = mysql_query($sql);

        if($row = mysql_fetch_assoc($result)) {
            require_once 'GameVersion.php';
            $parent = new GameVersion();
            $parent->loadFromDb($row['parent_game_version']);
            $this->name = $parent->name;
            $this->platform = $parent->platform;
            $this->region = $parent->region;
            
        }        
        
        
        
    }
    
    function loadFromXml($node) {
        global $wgOut;
        foreach($node->attributes as $attribute) {
            switch($attribute->name) {
                case 'append':
                case 'detract':
                case 'platform_version':
                case 'read_only':
                case 'deprecated':
                    break;
                case 'name':
                    $this->name = $attribute->value;
                    break;
                case 'platform':
                    $this->platform = $attribute->value;
                    break;
                default:
                    throw new Exception($attribute->name.' not supported');
            }
        }
        
        $wgOut->addHTML('<tr><td>');
        $wgOut->addHTML($this->name.'|'.$this->platform.'|'.$this->region);
        parent::loadFromXml($node);
        $wgOut->addHTML('</td></tr>');
    }
    
    public function writeToDb($id) {
        global $wgOut;
        $wgOut->addWikiText('*** Writing game parent ' . $this->name. '>'.$this->platform.'>'.$this->region.' to database');

        $criteria = array();
        $criteria[0] = 'name = \'' . $this->name . '\'';

        if ($this->platform == null) {
            $criteria[1] = 'platform = \'UNIVERSAL\'';
        } else {
            $criteria[1] = 'platform = \'' . $this->platform . '\'';
        }

        if ($this->region == null) {
            $criteria[2] = 'region = \'UNIVERSAL\'';
        } else {
            $criteria[2] = 'region = \'' . $this->region . '\'';
        }

        $dbw = wfGetDB(DB_SLAVE);
        $res = $dbw->select('masgau_game_data.game_versions', 'id', // $vars (columns of the table)
                        $criteria, __METHOD__, // $fname = 'Database::select',
                        SQL_NO_CACHE
                    );

        $parent_id = $res->fetchObject()->id;

        if($parent_id==null) {
            throw new Exception("Parent '. $this->name. '>'.$this->platform.'>'.$this->region.' not found in database. Make sure a parent is inserted BEFORE its children!");
        }
        
        $insert = array('parent_game_version'=>$parent_id);
        
        $this->writeAllToDb($id,'masgau_game_data.game_parents', $insert);

    }    
}

?>
