fx_version 'cerulean'
games { 'gta5' }

author 'Rijad Spahic'
description 'racegamemode'
version '1.0.0'

ui_page 'index.html'
files {
    'Newtonsoft.Json.dll',
    'index.html',
    'index.js'
}

client_scripts {
    'RaceClient.net.dll'
}
server_scripts {
    'RaceServer.net.dll'
}

