# RKD-TD

TODOs:

1. Prepare texture atlases: enemies, turrets, maps (including the starting and ending portals)
2. Design at least one map. Learn to save and load it.
3. Viewport: all maps should start with camera in the middle. Create LARGE map to hone that func.
4. Pathing is implemented through waypoints. Better I guess is to have a path on map and then build waypoints
   programmatically.
5. All could have animations mb even map tiles but later.
6. BASIC TILE SIZE IS 180! To only scale it downwards!
7. And we start at basic scale 1x.
8. Add simple monster generator and teach the game to end when health 0
9. Then add scaler (thoughs: viewport at the middle, get %, shrink and return to shrinked distance.)
10. Then develop and test towers one by one. Roadmap: add turret -> develop buying/purchasing/upgrading system -> damage
    system, enemies damaging and dying -> develop other towers

TILES Types:
LEGEND

* E = used by enemies
* T = used by turrets

1. Field = 00
5. Sand 01
2. Road E = 02
6. River 03
3. Bridge E Vertical 04
4. Platform T (05)
3. Bridge E Horizontal 06
7. Portal Starting E
8. Portal Ending E - these 2 are not tiles but objects
