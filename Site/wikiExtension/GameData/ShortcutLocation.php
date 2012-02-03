<?php

require_once 'Location.php';
class ShortcutLocation extends Location {
    //put your code here
    
    public $shortcut;
    
    public function loadFromDb($id) {
        $sql = 'select * from masgau_game_data.game_shortcuts where id = '.$id.'';
        $result = mysql_query($sql);
        
        if($row = mysql_fetch_assoc($result)) {
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
                case 'shortcut':
                    $this->shortcut = $attribute->value;
                    break;
                default:
                    throw new Exception($attribute->name.' not supported');
            }
        }
        
        $wgOut->addHTML('<tr><td>');
        $wgOut->addHTML($this->shortcut.'|');
        parent::loadFromXml($node);
        $wgOut->addHTML('</td></tr>');
    }

    public function writeToDb($id) {
        global $wgOut;
        $wgOut->addWikiText('*** Writing shortcut ' . $this->shortcut.' to database');

        $insert = array('shortcut'=>$this->shortcut);
        
        $this->writeAllToDb($id,'masgau_game_data.game_shortcuts', $insert);

    }        
    
}

?>
