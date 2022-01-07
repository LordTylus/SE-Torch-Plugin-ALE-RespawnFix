### Introduction
Many of you surely have noticed that the new optimal spawn feature ingame is not that well implemented, as players seem to randomly spawn close to players which have no relation to their faction, or are even enemies.

This plugins job is to fix that.

### How does it work?

Basically It makes sure the optimal spawn distance only applies if the player is part of a faction and the faction has characters (online and offline) in the world. 

It selects randomly one of the faction members. If the player has no faction or no faction member is available he will be dropped off randomly in the world. This MAY be close to a player but if it is its completely random. 

The location where the player spawns at then will be determined by the same logic players spawn with a space suit which never is close to any player. So the ranges can differ from 1km to 10,000km. 

Alternatively you also have to possibility to set a range where players shall spawn for example at least 500km away from world center but at most 3500km away. Of course configurable. It is disabled by default

### Configuration and Commands

In the config you can set up if you will be using the spawning radius as alterantive and how the ranges min and max are. 

### Github
[https://github.com/LordTylus/SE-Torch-Plugin-ALE-RespawnFix](https://github.com/LordTylus/SE-Torch-Plugin-ALE-RespawnFix)