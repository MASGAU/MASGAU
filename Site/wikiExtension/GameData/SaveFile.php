<?php 

/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 * Description of File
 *
 * @author TKMAYN9
 */
class SaveFile {
    public $mode;
    public $path;
    public $name;
    public $type;
    public $modified_after;

    function loadFromDb($row) {

        $this->mode = $row['action'];
        $this->path = $row['path'];
        $this->name = $row['filename'];
        $this->type = $row['type'];
        if($row['modified_after']!=null) {
            $date = new DateTime($row['modified_after']);
            $this->modified_after = $date;
        }
    }
    
    function loadFromXml($node) {
        global $wgOut;
        switch($node->localName) {
            case 'save':
                $this->mode = 'SAVE';
                break;
            case 'identifier':
                $this->mode = 'IDENTIFIER';
                break;
            case 'ignore':
                $this->mode = 'IGNORE';
                break;
            default:
                throw new Exception($node->localName.' not supported');
        }
        
        foreach($node->attributes as $attribute) {
            switch($attribute->name) {
                case 'path':
                    $this->path = $attribute->value;
                    break;
                case 'filename':
                    $this->name = $attribute->value;
                    break;
                case 'type':
                    $this->type = $attribute->value;
                    break;
                case 'modified_after':
                    $this->modified_after = $attribute->value;
                    break;
                default:
                    throw new Exception($attribute->name.' not supported');
            }
        }
        $wgOut->addHTML('<tr><td>'.$this->mode.'|'.$this->path.'|'.$this->name.'</td></tr>');
    }
    
    public function writeToDb($id) {
        global $wgOut;
        $dbw = wfGetDB(DB_MASTER);
        $wgOut->addWikiText('*** Writing '.$this->mode.' file ' . $this->path. '\\'.$this->name.' ('.$this->type.') to database');

        $insert = array('game_version'=>$id,'action'=>$this->mode);
        if($this->modified_after!=null)
                $insert['modified_after']= $this->modified_after;
        if($this->path!=null)
                $insert['path']= $this->path;
        if($this->name!=null)
                $insert['filename']= $this->name;
        if($this->type!=null)
                $insert['type']= $this->type;
        
        $dbw->insert('masgau_game_data.files', $insert, 
                $fname = 'Database::insert', $options = array());

    }
    
    
}

?>
