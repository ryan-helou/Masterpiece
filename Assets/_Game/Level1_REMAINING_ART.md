# Level 1 (Starry Night) — remaining art to generate

ALL of these on a **flat magenta `#ff00ff`** background (NOT a checkerboard). Save with the **exact
filenames** below into `Assets/_Game/Resources/Level1/` (overwrite the old checkerboard ones). The loader
auto-detects magenta now — just drop them in, stop Play, let it recompile, and Play. No code changes.

Shared style: thick swirling Van Gogh *Starry Night* impasto oil brushwork; deep cobalt/ultramarine blues
with glowing chrome-yellow `#ffd23a`, cream `#fff4d0` and cerulean `#6fb7ff`. Single centered object, no
text, no border.

---

## 1. `bg-moon.jpeg`  *(1:1 square)*
> A glowing crescent moon, thick chrome-yellow `#ffd23a` and cream `#fff4d0` impasto with concentric
> swirling halo rings radiating outward into a soft gold glow that fades off at the edges. Centered. Van
> Gogh *Starry Night* brushwork. **Flat magenta `#ff00ff` background — NOT a checkerboard.** No text.

## 2. `bg-star.jpeg`  *(1:1 square)*
> A single radiant night-sky star: a bright cream `#fff4d0` core, a glowing chrome-yellow `#ffd23a` halo
> ring, and short swirling brushstroke rays curling around it, with a soft bloom fading out. Centered. Van
> Gogh *Starry Night* brushwork. **Flat magenta `#ff00ff` background — NOT a checkerboard.** No text.

## 3. `bg-swirl.jpeg`  *(1:1 square)*
> A large swirling sky-vortex of long curling cobalt `#1b3a6b`, cerulean `#6fb7ff` and cream `#fff4d0`
> brushstrokes spiraling around an open center, luminous, built to slowly rotate behind the level.
> Circular, centered. **Flat magenta `#ff00ff` background, including the open center — NOT a checkerboard.**
> No text.

## 4. `pool.jpeg` — "The Void Below" (death pool, replaces the spikes)  *(wide ~2:1, horizontally SEAMLESS)*
> A churning pool of liquid night for a platformer death-pit — deep swirling cobalt and near-black blues
> caught in violent Van Gogh swirl-currents and eddies, with glowing cyan and chrome-yellow `#ffd23a`
> star-flecks swept into the spirals, and a bright turbulent surface line of cyan-white light along the
> very top edge that darkens to black toward the bottom. Ominous and deadly — like falling in would
> swallow you into the night sky — yet beautiful, thick impasto. The pool fills the whole tile below a
> thin strip of flat magenta at the very top (the air above the surface). The LEFT and RIGHT edges must
> match exactly so it tiles into one continuous pool with no seam. **Flat magenta `#ff00ff` background only
> above the surface line.** No spikes, no creatures, no text.

---

When all four are in `Resources/Level1/`: stop Play → recompile → Play. The moon/star/swirl will have clean
glows (no grid), and the spikes become a continuous swirling void.
