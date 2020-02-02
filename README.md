# EventParserForFacebook
EventParserForFacebook is a parser for the event section of Facebook Pages (e.g. [GitHub events on Facebook](https://de-de.facebook.com/pg/GitHub/events/?ref=page_internal)).

EventParserForFacebook parses the HTML-Structure of the Facebook pages and extracts the events as JSON or HTML. Optionally the HTML can be uploaded to a Worpress website. You can scan multiple pages at once and publish all of the events on one WordPress webiste.

# Build
[![Build status](https://dev.azure.com/jannis-muehlemeyer/Default/_apis/build/status/FacebookEventParser)](https://dev.azure.com/jannis-muehlemeyer/Default/_build/latest?definitionId=15)

# Why
Imagine that you have multiple subdivisions in your association and all of them publish their events on their own Facebook pages. You have the lucky task to inform your members about all new events. You could visit the Facebook pages regularly and grab the events.

Or you use this tool and generate a dashboard about all events on one website with automatic updates :)

# HowTo Use
1. Download Release binary (avaible for Windows x64 or linux-arm e.g. RaspberryPi)
2. Start FacebookEventParser once. An emtpy config file will be generated
3. Modfiy the config file (see section below)
4. Run FacebookEventParser regulary

## HowTo Config
The config is located next to the executables. It is called 'config.yaml'
```YAML
FacebookWebsites:
- NameDesVerbandes: Stadt Dortmund
  NameDerFacebookPage: dortmund
- NameDesVerbandes: Zoo Dortmund
  NameDerFacebookPage: ZooDortmund
WriteEventsAsHtmlToFile: true
WriteEventsAsJsonToFile: true

UploadEventsToWordpressWebsite: false
WordPressCredentials:
  Username: InsertWordpressUsernameHere
  Password: InsertWordpressPasswordHere
  BaseUrl: InsertWordpressUrlHere
WordpressPageId: 0

EnableTelegramBotIntegration: false
TelegramBotToken: 
```
## config.yaml
| Setting Name                   | Explanation                                                                                                                              |
|--------------------------------|------------------------------------------------------------------------------------------------------------------------------------------|
| FacebookWebsites               | The list of Facebook events pages which the parser should visit. You can add multiple pages here.                                        |
| - NameDesVerbandes             | Display Name for the Facebook Page in the generated HTML                                                                                 |
| - NameDerFacebookPage          | Name of the Facebook Event page. Example: You want to parse https://de-de.facebook.com/pg/GitHub/events/ so you should enter GitHub here |
| WriteEventsAsHtmlToFile        | true: An HTML-File is generated. Contains all events in a HTML-Format                                                                    |
| WriteEventsAsJsonToFile        | true: An .json-File is generated. Contains all events in a JSON-Format                                                                   |
| UploadEventsToWordpressWebsite | true: Uploads the HTML to your WordPress-Website                                                                                         |
| WordPressCredentials           | Necessary if UploadEventsToWordpressWebsite is set to true                                                                               |
| - Username                     | WordPress Username                                                                                                                       |
| - Password                     | WordPress Password                                                                                                                       |
| - BaseUrl                      | URL of the front page of your WordPress Website                                                                                          |
| WordpressPageId                | If of the page where your events should be published. Necessary if UploadEventsToWordpressWebsite is set to true                         |
| EnableTelegramBotIntegration   | true: You will get a notification on [Telegram](https://telegram.org/) when the parser is running :)                                                              |
| TelegramBotToken               | Telegram Token from the [Bot Father](https://core.telegram.org/bots#6-botfather). Necessary if UploadEventsToWordpressWebsite is set to true                                                                  |

# Output
## JSON-Output
```JSON
[
  {
    "AssociationName": "Stadt Dortmund",
    "FacebookDesktopUrl": "https://facebook.com/pg/dortmund/events/",
    "Events": [
      {
        "Title": "RUND UMS U - Die Dortmunder Partynacht! Samstag, 14. März 2020",
        "TimeStart": "2020-03-14T21:00:00",
        "Location": "Dortmund - Meine Stadt",
        "City": "Dortmund"
      },
      {
        "Title": "Stadtfest DortBunt! 2020",
        "TimeStart": "2020-05-10T00:00:00",
        "Location": "Dortbunt",
        "City": "Dortmund"
      }
    ]
  },
  {
    "AssociationName": "Zoo Dortmund",
    "FacebookDesktopUrl": "https://facebook.com/pg/ZooDortmund/events/",
    "Events": []
  }
]
```

## HTML-Output
```HTML
<div class="container">
<div class="row">
<div class="col-sm-6 inner">
<h3>RUND UMS U - Die Dortmunder Partynacht! Samstag, 14. März 2020</h3>
<div class="text-left"><span class="h4 small">Datum:</span> 14.03.2020</div>
<div class="text-left"><span class="h4 small">Uhrzeit:</span> 21:00</div>
<div class="text-left"><span class="h4 small">Ort:</span> Dortmund, Dortmund - Meine Stadt</div>
<div class="text-left"><span class="h4 small">Veranstalter:</span> Stadt Dortmund</div>
<div class="text-left"><span class="h4 small">Weitere Infos:</span> <a href="https://facebook.com/pg/dortmund/events/" target="_blank">Auf Facebook</a></div>
</div>
<div class="col-sm-6 inner">
<h3>Stadtfest DortBunt! 2020</h3>
<div class="text-left"><span class="h4 small">Datum:</span> 10.05.2020</div>
<div class="text-left"><span class="h4 small">Uhrzeit:</span> Keine Information</div>
<div class="text-left"><span class="h4 small">Ort:</span> Dortmund, Dortbunt</div>
<div class="text-left"><span class="h4 small">Veranstalter:</span> Stadt Dortmund</div>
<div class="text-left"><span class="h4 small">Weitere Infos:</span> <a href="https://facebook.com/pg/dortmund/events/" target="_blank">Auf Facebook</a></div>
</div>
</div>
<br><div class="text-muted small">Diese Übersicht fasst die Termine von Stadt Dortmund und Zoo Dortmund zusammen.</div>
```
