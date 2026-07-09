# 🖼️ Masterpiece

*A 2D speedrun platformer where each level is a famous painting you jump into and run against the clock.*

![Unity](https://img.shields.io/badge/Unity-000000?style=flat-square&logo=unity&logoColor=white) ![C#](https://img.shields.io/badge/C%23-512BD4?style=flat-square&logo=csharp&logoColor=white)

You walk a little avatar across a painted overworld map, stop at a landmark, and press jump to fall into the canvas. Inside, movement is the whole game. A high-precision timer arms the moment the level loads and starts the instant you move; cross the goal and it freezes to `M:SS.mmm`, saved to a local top-ten ladder with gold and silver medal targets per level. Your fastest run is recorded and replayed as a translucent ghost, and the HUD shows a live +/- against it as you go. A final pavilion, unlocked once every painting is cleared, starts a full-gallery run — every level back to back, one cumulative clock, with splits.

The point of it all is the controller. Gravity is hand-rolled (`gravityScale = 0`) so the jump arc is fully under the game's control: variable jump height, 0.09s of coyote time, a 0.10s jump buffer, a brief apex hang, fast-fall, a 0.12s dash on a 0.42s cooldown, wall jumps, corner-correction that slips you past clipped ledges, a momentum-preserving slide, and a Mario-style ground-pound. Top run speed is 11 units/s; a jump clears 3.3. Stomping enemies and chaining dashes builds a combo that resets when you touch ground.

Each canvas also sets the level's signature mechanic:

- Starry Night (van Gogh, 1889) — dreamy low gravity; tap jump inside a swirl-ring for a second boost, then glide the long gaps.
- The Great Wave off Kanagawa (Hokusai, 1831) — wave-pads fling you and spray pearls refill your air-dash; the blue sea is death.
- The Swing (Fragonard, 1767) — board a ridden pendulum seat at one apex and leap from the other, riding fountain-jet lifts up the garden.
- The Scream (Munch, 1893) — crumbling bridge boards drop the instant you touch them, and the level ends fighting the Scream itself: dodge its plunges, stomp its crown three times.

The whole thing is built code-first. Drop a single `GameBootstrap` component on one GameObject and press Play — camera, input, UI, state machine and every level construct themselves at runtime. Levels are authored as char grids in code rather than hand-placed scenes, physics runs at 120 Hz, and a pixelation pass gives it a retro look. Palettes and hand-built scenery are sampled per painting; the real artwork only ever appears small, framed on the overworld.

Built in Unity 6000.4.10f1 with URP and the Input System. Mid-development: four of a planned eight paintings are playable so far.
