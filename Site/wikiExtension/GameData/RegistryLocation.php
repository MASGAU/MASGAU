<?php

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of RegistryLocation
 *
 * @author TKMAYN9
 */
require_once 'Location.php';
class RegistryLocation extends Location {
    //put your code here
    
    public $root;
    public $key;
    public $value = null;
    
    public function loadFromDb($id) {
        $sql = 'select * from masgau_game_data.game_registry_keys where id = '.$id.'';
        $result = mysql_query($sql);
        
        if($row = mysql_fetch_assoc($result)) {
            $this->root = $row['root'];
            $this->key = $row['key'];
            $this->value = $row['value'];
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
                case 'root':
                    $this->root = $attribute->value;
                    break;
                case 'key':
                    $this->key = $attribute->value;
                    break;
                case 'value':
                    $this->value= $attribute->value;
                    break;
                default:
                    throw new Exception($attribute->name.' not supported');
            }
        }
        $wgOut->addHTML('<tr><td>');
        $wgOut->addHTML($this->root.'|'.$this->key.'|'.$this->value.'|');
        parent::loadFromXml($node);
        $wgOut->addHTML('</td></tr>');
    }
    
    public function writeToDb($id) {
        global $wgOut;
        $wgOut->addWikiText('*** Writing registry key ' . $this->root. '\\'.$this->key.'\\'.$this->value.' to database');

        $insert = array('root'=>$this->root,'`key`'=>$this->key);
        if($this->value!=null)
                $insert['value']= $this->value;
        
        $this->writeAllToDb($id,'masgau_game_data.game_registry_keys', $insert);
    }    
}

?>
