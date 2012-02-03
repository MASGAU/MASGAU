<?php

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of Location
 *
 * @author TKMAYN9
 */
class Location {
    public $append = null;
    public $detract = null;
    public $platform_version = null;
    public $deprecated = false;
    
    
    protected function loadFromDb($id) {
        $sql = 'select * from masgau_game_data.game_locations where id = '.$id.'';
        $result = mysql_query($sql);
        
        if($row = mysql_fetch_assoc($result)) {
            $this->append = $row['append'];
            $this->detract = $row['detract'];
            $this->platform_version = $row['platform_version'];
            $this->deprecated = $row['deprecated'];
        }        
    }
    
    protected function loadFromXml($node) {
        global $wgOut;
        foreach($node->attributes as $attribute) {
            switch($attribute->name) {
                case 'append':
                    $this->append = $attribute->value;
                    break;
                case 'detract':
                    $this->detract = $attribute->value;
                    break;
                case 'platform_version':
                    $this->platform_version = $attribute->value;
                    break;
                case 'read_only':
                case 'deprecated':
                    if($attribute->value=="true")
                        $this->deprecated = true;
                    break;
                default:
                    //throw new Exception($attribute->name.' not supported');
            }
        }
        $wgOut->addHTML('Append:'.$this->append.'|Detract:'.$this->detract.'|Platform Ver.:'.$this->platform_version.'|Deprecated:'.$this->deprecated);
    }

    protected function writeAllToDb($id,$table,$sub_insert) {
        $dbw = wfGetDB(DB_MASTER);
        global $wgOut;
        $wgOut->addWikiText('**** Additional Location write: Append: ' . $this->append. ' Detract: '.$this->detract.' Platform Ver.: '.$this->platform_version.' Deprecated: '.$this->deprecated);
        

        $res = $dbw->select('masgau_game_data.game_locations', 'max(id) max', // $vars (columns of the table)
                null, __METHOD__, // $fname = 'Database::select',
                SQL_NO_CACHE
         );
        
        $loc_id = $res->fetchObject()->max;
        $loc_id++;
        
        $insert = array('game_version'=>$id,'id'=>$loc_id);
        
        
        if($this->append!=null) 
                $insert['append']= $this->append;
        if($this->detract!=null)
                $insert['detract']= $this->detract;
        if($this->platform_version!=null)
                $insert['platform_version']= $this->platform_version;
        if($this->deprecated!=null)
                $insert['deprecated']= $this->deprecated;
        
        
        $dbw->insert('masgau_game_data.game_locations', $insert, 
                $fname = 'Database::insert', $options = array());
        

        $sub_insert['id'] = $loc_id;
        
        $dbw->insert($table, $sub_insert, 
                $fname = 'Database::insert', $options = array());
        
        }            
}

?>
