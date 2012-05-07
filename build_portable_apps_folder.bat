mkdir MASGAU.Portable\App\MASGAU
mkdir MASGAU.Portable\App\MASGAU\Data
mkdir MASGAU.Portable\Data

copy MASGAU.Main.WPF\bin\Release\* MASGAU.Portable\App\MASGAU
del MASGAU.Portable\App\MASGAU\*.vshost.*
del MASGAU.Portable\App\MASGAU\data.*

move MASGAU.Portable\App\MASGAU\config.xml MASGAU.Portable\App\DefaultData

copy Data\Data\*.x* MASGAU.Portable\App\MASGAU\Data\
del MASGAU.Portable\App\MASGAU\Data\!*
