<?php

require_once 'Location.php';
class ShortcutLocation extends Location {
    //put your code here
    
    public $ev;
    public $shortcut;
    
    public function loadFromDb($id) {
        $sql = 'select * from masgau_game_data.game_shortcuts where id = '.$id.'';
        $result = mysql_query($sql);
        
        if($row = mysql_fetch_assoc($result)) {
            $this->ev = $row['ev'];
            $this->shortcut = $row['shortcut'];
        }        
        parent::loadFromDb($id);
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
                case 'environment_variable':
                    $this->ev = $attribute->value;
                    break;
                case 'shortcut':
                    $this->shortcut = $attribute->value;
                    break;
                default:
                    throw new Exception($attribute->name.' not supported');
            }
        }
        
        $wgOut->addHTML('<tr><td>');
        $wgOut->addHTML($this->ev.'|'.$this->shortcut.'|');
        parent::loadFromXml($node);
        $wgOut->addHTML('</td></tr>');
    }

    public function writeToDb($id) {
        global $wgOut;
        $wgOut->addWikiText('*** Writing shortcut ' . $this->ev. '\\'.$this->shortcut.' to database');

        $insert = array('ev'=>$this->ev,'shortcut'=>$this->shortcut);
        
        $this->writeAllToDb($id,'masgau_game_data.game_shortcuts', $insert);

    }        
    
}

?>
