<?php

class SpecialGameData extends SpecialPage {

    function __construct() {
        parent::__construct('GameData');
    }

    function execute($par) {
        global $wgRequest, $wgOut;

        $this->setHeaders();

        
        
        $par = $wgRequest->getText('title');
        $bits = explode( '/', trim( $par ) );
        
        # Get request data from, e.g.
        if (sizeof($bits[1])>0) {
            $game = $bits[1];
        } else {
            $game = 'DeusEx';
        }

        require_once 'GameData/Game.php';
        $game_data = new Game();
        $game_data->loadFromDb($game, null);


        $game_title = $game_data->title;

        $wgOut->setPagetitle("Game Data - " . $game_title);

        $dbr = wfGetDB(DB_SLAVE);

        require_once('CompatabilityTable.php');


        $wgOut->addWikiText('<h2>MASGAU Compatibility</h2>');
        beginCompatTable();
        drawCompatHeader();
        $res = $dbr->select('masgau_game_data.current_compatibility', array('*'), // $vars (columns of the table)
                        'game = \''    . $game . '\'', // $conds
                        __METHOD__, // $fname = 'Database::select',
                        array('ORDER BY' => 'game ASC')
        );
        if ($res->numRows() > 0) {
            drawCompatRow($res->fetchObject(), 'Current');
        }
        $res = $dbr->select('masgau_game_data.upcoming_compatibility', array('*'), // $vars (columns of the table)
                        'game = \''    . $game . '\'', // $conds
                        __METHOD__, // $fname = 'Database::select',
                        array('ORDER BY' => 'game ASC')
        );
        if ($res->numRows() > 0) {
            drawCompatRow($res->fetchObject(), 'Upcoming');
        }
        endCompatTable();


        foreach ($game_data->versions as $version) {
            $anchor = '' . $platform . $region;
            if ($version->region != null) {
                if ($version->platform != null) {
                    $header = $version->platform . ' - ' . $version->region;
                } else {
                    $header = $version->region;
                }
            } else {
                if ($version->platform != null) {
                    $header = $version->platform;
                } else {
                    $header = 'Platform Neutral';
                }
            }
            $header .=' Version';

            if ($version->deprecated)
                $header .= ' (Deprecated)';

            if ($row->version_title != null)
                $header .= ' (' . $row->version_title . ')';

            $wgOut->addWikiText('<h2>' . $header . '</h2>');
            $wgOut->addHTML('<a name="' . $anchor . '" />');

            // Side info box
            $wgOut->addHTML('<div style="float:right;border:solid 1px;padding:5px;width:200px;" class="wikitable">');
            if ($version->platform != null) {
                $wgOut->addHTML('Platform: ' . $version->platform . '<br />');
            }
            if ($version->region != null) {
                $wgOut->addHTML('Region: ' . $version->region . '<br />');
            }

            if ($version->deprecated) {
                $wgOut->addHTML('This game does not recognize VirtualStore folders.<br />');
            }
            if ($version->require_detection) {
                $wgOut->addHTML('Restoring this game requires detecting saves taht are already present.<br />');
            }

            $wgOut->addHTML('Contributors:<br />');
            foreach ($version->contributors as $contributor) {
                $wgOut->addWikiText('* [[Special:Contributor/' . $contributor.'|' . $contributor.']]');
            }
            $wgOut->addHTML('</div>');

            if ($version->deprecated) {
                $wgOut->addHTML('This game version is no longer supported for backup. Archvies previously made for this version can still be restored.<br />');
            }

            //PSP codes
            foreach ($version->ps_codes as $path) {
                $wgOut->addWikiText('* ' . $path->prefix . '-' . $path->suffix);
            }

            // Paths
            if (sizeof($version->locations) > 0) {
                $wgOut->addWikiText('<h3>Locations</h3>');
                $paths = null;
                $reg_keys = null;
                $shortcuts = null;
                $parent_games = null;
                foreach ($version->locations as $location) {
                    $line = '';
                    if (get_class($location) != "PathLocation") {
                        if ($location->append != null)
                            $line = '<td>' . $location->append . '</td>';
                        else
                            $line = '<td></td>';

                        if ($location->detract != null)
                            $line .= '<td>' . $location->detract . '</td>';
                        else
                            $line .= '<td></td>';
                    }

                    if ($location->platform_version != null)
                        $line = '<td>Only works with ' . $location->platform_version . '</td>';

                    if ($location->platform_version != null)
                        $line = '<td>Only works with ' . $location->platform_version . '</td>';

                    if ($location->deprecated)
                        $line .= '<td style="background-color:red">Deprecated</td>';

                    switch (get_class($location)) {
                        case "PathLocation":
                            if ($paths == null) {
                                $paths = '<table class="wikitable"><caption>Paths</caption><tr><th>Environment Variable</th><th>Path</th></tr>';
                            }
                            $paths .= '<tr><td>' . $location->ev . '</td><td>' . $location->path . '</td>' . $line . '</tr>';
                            break;
                        case "RegistryLocation":
                            if ($reg_keys == null) {
                                $reg_keys = '<table class="wikitable"><caption>Registry Keys</caption><tr><th>Root</th><th>Key</th><th>Value</th><th>Append</th><th>Detract</th></tr>';
                            }
                            $reg_keys .='<tr><td>' . $location->root . '</td><td>' . $location->key . '</td>';
                            if ($location->value == null)
                                $reg_keys .='<td>(Default)</td>';
                            else
                                $reg_keys .='<td>' . $location->value . '</td>';
                            $reg_keys .= $line . '</tr>';
                            break;
                        case "ShortcutLocation":
                            if ($shortcuts == null) {
                                $shortcuts = '<table class="wikitable"><caption>Shortcuts</caption><tr><th>Path</th><th>Append</th><th>Detract</th></tr>';
                            }
                            $shortcuts .= '<tr><td>' . $location->shortcut . '</td>' . $line . '</tr>';
                            break;
                        case "GameLocation":
                            if ($parent_games == null) {
                                $parent_games = '<table class="wikitable"><caption>Parent Game Versions</caption><tr>
                                    <th>Game</th><th>Platform</th><th>Region</th><th>Append</th><th>Detract</th></tr>';
                            }
                            $parent_games .= '<tr><td>[[Special:GameData/' . $location->name . '|' . $location->name . ']]</td><td>' . $location->platform . '</td><td>' . $location->region . '</td>' . $line . '</tr>';
                            break;
                        default:
                            continue;
                    }
                }

                if ($paths != null)
                    $wgOut->addHTML($paths . '</table>');
                if ($reg_keys != null)
                    $wgOut->addHTML($reg_keys . '</table>');
                if ($shortcuts != null)
                    $wgOut->addHTML($shortcuts . '</table>');
                if ($parent_games != null)
                    $wgOut->addHTML($parent_games . '</table>');
            }



            $saves = null;
            $ignores = null;
            $identifiers = null;
            foreach ($version->files as $file) {

                $line = '<tr>';
                if ($file->mode == 'IDENTIFIER') {
                    $line .= '<td>' . $file->path . '</td>';
                    $line .= '<td>' . $file->name . '</td>';
                } else {
                    if ($file->name == null)
                        $filename = "* (Includes subfolders)";
                    else
                        $filename = $file->name;

                    $line .= '<td>' . $file->path . '</td>';
                    $line .= '<td>' . $filename . '</td>';
                    $line .= '<td>' . $file->type . '</td>';
                    $line .= '<td>' . $file->modified_after . '</td>';
                }
                $line .= '</tr>';

                switch ($file->mode) {
                    case 'SAVE':
                        if ($saves == null)
                            $saves = '<table class="wikitable"><caption>To Save</caption><tr><th>Path</th><th>Filename</th><th>Type</th><th>Modified After</th></tr>';
                        $saves .= $line;
                        break;
                    case 'IGNORE':
                        if ($ignores == null)
                            $ignores = '<table class="wikitable"><caption>To Ignore</caption><tr><th>Path</th><th>Filename</th><th>Type</th><th>Modified After</th></tr>';
                        $ignores .= $line;
                        break;
                    case 'IDENTIFIER':
                        if ($identifiers == null)
                            $identifiers = '<table class="wikitable"><caption>Used To Identify Game</caption><tr><th>Path</th><th>Filename</th></tr>';
                        $identifiers .= $line;
                        break;
                }
            }

            if ($saves != null || $ignores != null || $identifiers != null) {
                $wgOut->addWikiText('<h3>Files</h3>');
                if ($identifiers != null)
                    $wgOut->addHTML($identifiers . '</table>');
                if ($saves != null)
                    $wgOut->addHTML($saves . '</table>');
                if ($ignores != null)
                    $wgOut->addHTML($ignores . '</table>');
            }

            if ($version->comment != null) {
                $wgOut->addWikiText('<h3>Comment</h3>');
                $wgOut->addWikiText($version->comment);
            }

            if ($version->restore_comment != null) {
                $wgOut->addWikiText('<h3>Restore Comment</h3>');
                $wgOut->addWikiText($version->restore_comment);
            }

        $res = $dbr->select(array('ver'=>'masgau_game_data.xml_versions',
            'ex'=>'masgau_game_data.exporters'), 
                array('*'), // $vars (columns of the table)
                        'ver.exporter = ex.name', // $conds
                        __METHOD__, // $fname = 'Database::select',
                        array('ORDER BY' => 'major desc, minor desc')
        );

            $row = $res->fetchObject();
            require_once 'Exporters/'.$row->file;

            $wgOut->addWikiText('<h3>MASGAU '.$row->string.' XML</h3>');
            $xml = new DOMDocument();
            $xml->encoding = 'utf-8';
            $xml->formatOutput = true;
            $exporter = new Exporter();
            $exporter->xml = $xml;
            $node = $exporter->exportGameVersion($game_data, $version);
            $xml->appendChild($node);
            $wgOut->addWikiText('<syntaxhighlight style="margin-right:250px;" lang="xml" class="mw-collapsible" enclose="div">' .
                    $xml->saveXML($node) .
                    '</syntaxhighlight>');
        }
    }

}