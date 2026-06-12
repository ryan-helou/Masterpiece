# Audio Design — the generative score & SFX system

Everything is synthesized at runtime; the game still ships with zero audio assets. This doc explains
what plays, why it's built this way, and which knobs to turn when something feels off.

## Why the old audio was failing

- **One fixed 8-second loop** played identically on the menu and all four levels. Research consensus:
  loops under ~30s become consciously annoying; 60–90s is the practical floor — and a *generative*
  score that never repeats verbatim is the strongest anti-fatigue weapon a no-assets game has.
- **Every SFX was bit-identical on every play.** A speedrunner hears the jump sound thousands of
  times; without per-shot pitch variance it turns grating.
- **Mechanics shared sounds** (spring = jump, note-block = jump, boost = dash, portal = wall-jump =
  dash-orb), so the ear couldn't learn the game's vocabulary.

## The music engine (`Music.cs`)

A tiny sequencer + synth runs live on the audio thread (`OnAudioFilterRead`). There is no loop:
a melody random-walks the level's scale over a slow chord progression, pulled onto chord tones on
strong beats, with rest probability and a "breath" bar every 8th bar so phrases keep air. Phrases
recombine forever — literal repetition never occurs.

**Each painting has its own score identity:**

| Level | Mood | Mode / meter | BPM | Character |
|---|---|---|---|---|
| Starry Night | `night` | A minor pentatonic, 4/4 | 66 | drifting music-box over a deep three-partial bed, long echo |
| Creation of Adam | `sacred` | D Mixolydian, 4/4 | 52 | slow organum — the melody sings in parallel fifths over an organ drone stack, vast delay |
| The Swing | `waltz` | F major, **3/4** | 104 | bright harpsichord-ish plucks, oom-pah-pah bass — the swing literally swings |
| The Scream | `anxious` | E Phrygian, 4/4 | 140 | driving low pulse; the drone's second partial is tuned ~25 cents sharp so it audibly *beats* — the dread is physical |
| Menu / Overworld | `gallery` | C major pentatonic | 56 | sparse piano-like notes, museum quiet |

**The arrangement is adaptive** (Celeste's layer architecture, simplified):

- **Drone bed** — always on. The painting's tonal floor.
- **Melody** — gain follows the player's *speed* (0.45→1.0). Flow is rewarded with music; standing
  still thins the score.
- **Pulse layer** — fades in across 45→70% level *progress* (a low chord-root heartbeat; in the
  waltz it's the bass of the oom-pah-pah). Mirrors the levels' act-3 escalation.
- **Shimmer** — two detuned high sines on the chord root, fading in at 70→90% progress: climax air.
- Layer gains glide slowly (~2–4s), Celeste-style — intensity creeps, never jumps.

**Death never restarts the music** (Meat Boy / Celeste / Hotline Miami rule — the track keeps
rolling so a death streak stays calm). Instead the score **ducks** ~5 dB for the death sting and
recovers over 0.7s; since death resets level progress, the pulse/shimmer layers naturally fall away
— intensity resets without the track ever stopping. The finish fanfare gets a longer, gentler duck.

**Frequency discipline:** drones live below 250 Hz, melodies in the comfortable mid-register, and
nothing sustained sits in the harsh 2–5 kHz band — that range is reserved for brief, rare events.

## The SFX engine (`Sfx.cs`)

- **Six-source pool with per-shot pitch variance** (±~50 cents, the researched sweet spot for tonal
  sounds): no two plays are ever identical, which is the single biggest anti-annoyance lever.
- **Every mechanic owns a distinct earcon** now: spring (sine boing), note-block (octave pluck),
  boost pad (noise+saw whoosh), dash orb (detuned crystal ping), portal (descending warp), jump ring
  (airy chime), crumble (filtered crack), falling platform (low creak warning — plays the instant it
  starts shaking, so it doubles as the hazard telegraph), ground-pound slam (whoosh + rumble).
- **Timbres softened**: the constantly-heard movement sounds moved from raw squares to triangles at
  lower gains; squares are reserved for punchy rare hits (stomp).
- **Pitch semiotics**: everything good rises (coin, fanfares, power-up, combo chain — which still
  climbs a semitone per stomp), everything bad falls (death, warp-out). The death sound is short,
  low, and mixed to never feel punishing on the 200th hearing.
- **Pickups are tuned to the score.** On every theme switch, `Music` calls `Sfx.Tune(root, scale)`
  and the coin, note-block, ring, orb, power-up and fanfare sounds are regenerated from the level's
  actual scale degrees — reward SFX land *inside* the music (Mini Metro's trick), so SFX-vs-music
  dissonance is structurally impossible.
- **Anti-stack guard**: a 30 ms per-sound cooldown stops doubled triggers from spiking.

## Knobs to turn

| Feels... | Turn |
|---|---|
| Music too busy | raise the mood's `RestProb`, lower `MelodyGain` |
| Music too empty | lower `RestProb`, raise `Bpm` slightly |
| Climax not landing | raise `PulseGain`/`ShimmerGain`, or move their progress windows (in `OnAudioFilterRead`) |
| Theme switch too abrupt | lower the `_fadeVel` constants (±1.2 / 0.8) |
| A mood's vibe is off | edit its `Scale`/`Chords` — everything downstream (melody, pulse, shimmer, tuned SFX) follows automatically |
| An SFX is annoying | lower its gain in `Awake`, or raise its pitch-variance argument |

## Not built yet (next tier)

- **Beat-locked hazards** (Rayman Legends rule): drive saw/pendulum phases from the music's step
  clock so mastered levels feel choreographed. Big payoff, medium plumbing.
- **Death-streak texture thinning**: after ~5 deaths in 30s, drop the melody layer for a while —
  the score gets gentler exactly when the player is hottest.
- **Low-pass dip on death** (in addition to the duck) — needs a filter on the music path.
- **Per-theme percussion voices** (brushed noise hats for the waltz, deep toms for the scream).
