<?php
error_reporting(E_ERROR | E_WARNING | E_PARSE);

# Alert the user that this is not a valid entry point to MediaWiki if they try to access the special pages file directly.
if (!defined('MEDIAWIKI')) {
        echo <<<EOT
To install my extension, put the following line in LocalSettings.php:
require_once( "\$IP/extensions/MASGAU/MASGAU.php" );
EOT;
        exit( 1 );
}
 
$wgExtensionCredits['specialpage'][] = array(
        'path' => __FILE__,
        'name' => 'MASGAU',
        'author' => 'Matthew Barbour',
        'url' => 'https://masgau.sf.net/',
        'descriptionmsg' => 'masgau-desc',
        'version' => '0.1.0',
);
 
$dir = dirname(__FILE__) . '/';
 
$wgAutoloadClasses['SpecialGames'] = $dir . 'SpecialGames.php'; # Location of the SpecialMyExtension class (Tell MediaWiki to load this file)
$wgAutoloadClasses['SpecialContributors'] = $dir . 'SpecialContributors.php'; # Location of the SpecialMyExtension class (Tell MediaWiki to load this file)
$wgAutoloadClasses['SpecialContributor'] = $dir . 'SpecialContributor.php'; # Location of the SpecialMyExtension class (Tell MediaWiki to load this file)
$wgAutoloadClasses['SpecialGameCompatibility'] = $dir . 'SpecialGameCompatibility.php'; # Location of the SpecialMyExtension class (Tell MediaWiki to load this file)
$wgAutoloadClasses['SpecialGameCompatibilityEditor'] = $dir . 'SpecialGameCompatibilityEditor.php'; # Location of the SpecialMyExtension class (Tell MediaWiki to load this file)
$wgAutoloadClasses['SpecialGameData'] = $dir . 'SpecialGameData.php'; # Location of the SpecialMyExtension class (Tell MediaWiki to load this file)
$wgAutoloadClasses['SpecialXMLImport'] = $dir . 'SpecialXMLImport.php'; # Location of the SpecialMyExtension class (Tell MediaWiki to load this file)
$wgAutoloadClasses['SpecialXMLExport'] = $dir . 'SpecialXMLExport.php'; # Location of the SpecialMyExtension class (Tell MediaWiki to load this file)

$wgExtensionMessagesFiles['MASGAU'] = $dir . 'MASGAU.i18n.php'; # Location of a messages file (Tell MediaWiki to load this file)
$wgExtensionMessagesFiles['MASGAUAlias'] = $dir . 'MASGAU.alias.php'; # Location of an aliases file (Tell MediaWiki to load this file)

$wgSpecialPages['Games'] = 'SpecialGames'; # Tell MediaWiki about the new special page and its class name
$wgSpecialPages['Contributors'] = 'SpecialContributors'; # Tell MediaWiki about the new special page and its class name
$wgSpecialPages['Contributor'] = 'SpecialContributor'; # Tell MediaWiki about the new special page and its class name
$wgSpecialPages['GameCompatibility'] = 'SpecialGameCompatibility'; # Tell MediaWiki about the new special page and its class name
$wgSpecialPages['GameCompatibilityEditor'] = 'SpecialGameCompatibilityEditor'; # Tell MediaWiki about the new special page and its class name
$wgSpecialPages['GameData'] = 'SpecialGameData'; # Tell MediaWiki about the new special page and its class name
$wgSpecialPages['XMLImport'] = 'SpecialXMLImport'; # Tell MediaWiki about the new special page and its class name
$wgSpecialPages['XMLExport'] = 'SpecialXMLExport'; # Tell MediaWiki about the new special page and its class name

$wgSpecialPageGroups['Games'] = 'masgau';
$wgSpecialPageGroups['GameCompatibility'] = 'masgau';
$wgSpecialPageGroups['GameCompatibilityEditor'] = 'masgau';
$wgSpecialPageGroups['GameData'] = 'masgau';
$wgSpecialPageGroups['XMLImport'] = 'masgau';
$wgSpecialPageGroups['XMLExport'] = 'masgau';
$wgSpecialPageGroups['Contributors'] = 'masgau';
$wgSpecialPageGroups['Contributor'] = 'masgau';
