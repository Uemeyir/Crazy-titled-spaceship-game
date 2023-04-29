using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using sopra05_2223.InputSystem;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using sopra05_2223.Core;
using sopra05_2223.Core.Animation;
using sopra05_2223.Serializer;
using SoundState = Microsoft.Xna.Framework.Audio.SoundState;

namespace sopra05_2223.SoundManagement;

internal enum SoundEnum
{
    Click1,
    Bombership,
    Shootership,
    Transportship,
    BaseExplosion,
    ShipExplosion,
    ShipExplosion2,
    ShipExplosion3,
    Shot,
    Shot2,
    Shot3,
    Heal,
    Heal2,
    Heal3,
    Woosh1,
    Collect,
    Collect2
}

internal enum MusicEnum
{
    Music1,
    Music2,
    Music3,
    Music4,
    MusicMenu,
}


public sealed class SoundManager
{
    private const int SoundLimit = 20;
    [JsonRequired]
    private float mSoundVolume = 0.5f;
    [JsonRequired]
    private float mMusicVolume = 0.5f;
    [JsonRequired]
    internal bool mSoundMuted;

    private readonly List<SoundEffectInstance> mRunningSounds;
    private readonly Dictionary<SoundEnum, SoundEffect> mLoadedSounds;
    private readonly Dictionary<SoundEnum, string> mSoundSource;

    private readonly Dictionary<MusicEnum, string> mMusicSource;
    private readonly Dictionary<MusicEnum, SoundEffect> mLoadedMusic;
    private SoundEffectInstance mCurrentMusic;
    private MusicEnum mCurrentMusicEnum;

    private HashSet<IAudible> mAudibles;

    private SoundEffectInstance mMoveSound;

    public SoundManager(ContentManager content)
    {
        mRunningSounds = new List<SoundEffectInstance>();


        // Create Sound enum, Source key-value pairs and add to SoundSource to load them on game init
        mSoundSource = new Dictionary<SoundEnum, string> {
            { SoundEnum.Click1, "Sounds/click1" },
            { SoundEnum.Bombership, "Sounds/chatter1" },
            { SoundEnum.Shootership, "Sounds/chatter2" },
            { SoundEnum.Transportship, "Sounds/chatter3" },
            { SoundEnum.BaseExplosion, "Sounds/knock" },
            { SoundEnum.ShipExplosion, "Sounds/explosion5" },
            { SoundEnum.ShipExplosion2, "Sounds/explosion5_1" },
            { SoundEnum.ShipExplosion3, "Sounds/explosion5_2" },
            { SoundEnum.Shot, "Sounds/general1" },
            { SoundEnum.Shot2, "Sounds/general1_1" },
            { SoundEnum.Shot3, "Sounds/general1_2" },
            { SoundEnum.Heal, "Sounds/general4" },
            { SoundEnum.Heal2, "Sounds/general4_1" },
            { SoundEnum.Heal3, "Sounds/general4_2" },
            { SoundEnum.Collect, "Sounds/cassette1" },
            { SoundEnum.Collect2, "Sounds/cassette2" },
            { SoundEnum.Woosh1, "Sounds/woosh1"}

        };
        mLoadedSounds = new Dictionary<SoundEnum, SoundEffect>();


        // Create Sound enum, Source key value pairs and add to MusicSource to load them on game init
        mMusicSource = new Dictionary<MusicEnum, string> {
            { MusicEnum.Music2, "Sounds/Anemoia - Exosphere" },
            { MusicEnum.Music1, "Sounds/Audiorezout - Millennium" },
            { MusicEnum.MusicMenu, "Sounds/Anemoia - Time" },
            { MusicEnum.Music4, "Sounds/Simon Mathewson - Landpad" },
            { MusicEnum.Music3, "Sounds/Viscid - Abyss" }

        };
        mLoadedMusic = new Dictionary<MusicEnum, SoundEffect>();

        mCurrentMusicEnum = MusicEnum.MusicMenu;

        Init(content);
    }

    [JsonConstructor]
    private SoundManager(float musicVolume, float soundVolume, bool soundMuted)
    {
        mMusicVolume = musicVolume;
        mSoundVolume = soundVolume;
        mSoundMuted = soundMuted;
    }


    // OPTIONAL: Can Later be optimized, so unused Sounds are not always loaded. reduces Memory requirement
    private void Init(ContentManager content)
    {
        // Load all sounds specified in mSoundSource
        foreach (var kvpair in mSoundSource)
        {
            mLoadedSounds.Add(kvpair.Key, content.Load<SoundEffect>(kvpair.Value));
        }

        //load music
        foreach (var kvpair in mMusicSource)
        {
            mLoadedMusic.Add(kvpair.Key, content.Load<SoundEffect>(kvpair.Value));
        }

        mAudibles = new HashSet<IAudible>();
    }

    internal void PlayMusic(MusicEnum track)
    {
        // this check is redundant, but might be relevant later on
        StopMusic();
        if (mCurrentMusic == null || mCurrentMusic.State == SoundState.Stopped)
        {
            mCurrentMusic = mLoadedMusic[track].CreateInstance();
            if (this.mMusicVolume <= 0)
            {
                this.mMusicVolume = 0f;
            }

            if (this.mMusicVolume >= 1)
            {
                this.mMusicVolume = 1f;
            }

            mCurrentMusic.Volume = this.mMusicVolume;
            if (mSoundMuted)
            {
                mCurrentMusic.Volume = 0;
            }
            mCurrentMusic.Play();
            mCurrentMusicEnum = track;
        }
    }

    // will be useful when changing from menu to in game
    private void StopMusic()
    {
        if (mCurrentMusic != null)
        {
            mCurrentMusic.Stop();
        }
    }

    public void Update(Input input)
    {
        RemoveEnded();

        if (mCurrentMusic == null || mCurrentMusic.State == SoundState.Stopped)
        {
            if (mCurrentMusicEnum == MusicEnum.MusicMenu)
            {
                PlayMusic(MusicEnum.MusicMenu);
                mCurrentMusicEnum = MusicEnum.MusicMenu;
            }
            else
            {
                var rand = (ushort)Globals.RandomNumber();
                mCurrentMusicEnum = mLoadedMusic.Keys.ToArray()[rand % mLoadedMusic.Keys.Count];
                if (mCurrentMusicEnum == MusicEnum.MusicMenu)
                {
                    mCurrentMusicEnum = MusicEnum.Music1;
                }
            }

        }

        var audibles = mAudibles.ToArray();

        for (var i = audibles.Length - 1; i >= 0; i--)
        {
            if (audibles[i].IsRemovable())
            {
                mAudibles.Remove(audibles[i]);
                continue;
            }

            var sounds = audibles[i].GetQueuedSound();
            var pan = audibles[i].GetPan();
            foreach (var sound in sounds)
            {
                PlaySoundAt(sound, pan);
            }

            audibles[i].ResetSound();
        }

        if (input.GetKey(Globals.mKeys["Mute"]))
        {
            mSoundMuted = !mSoundMuted;
            SetSoundVolume(mSoundVolume);
            SetMusicVolume(mMusicVolume);
            SopraSerializer.SerializeSettings();
        }
    }

    // Plays the sound associated with the enum
    internal void PlaySound(SoundEnum soundEnum)
    {
        RemoveEnded();
        if (mRunningSounds.Count < SoundLimit)
        {
            var instance = mLoadedSounds[soundEnum].CreateInstance();
            if (mSoundVolume <= 0)
            {
                mSoundVolume = 0;
            }

            if (mSoundVolume >= 1)
            {
                mSoundVolume = 1;
            }
            instance.Volume = mSoundVolume;
            if (mSoundMuted)
            {
                instance.Volume = 0;
            }
            
            if (soundEnum != SoundEnum.Shootership && soundEnum != SoundEnum.Transportship &&
                soundEnum != SoundEnum.Bombership)
            {
                mRunningSounds.Add(instance);
                instance.Play();
            }
            else
            {
                if (mMoveSound != null && mMoveSound.State != SoundState.Stopped)
                {
                    mMoveSound.Stop();
                }
                mMoveSound = instance;
                mMoveSound.Play();
            }
            
        }
    }

    internal void PlaySoundAt(SoundEnum soundEnum, float pan)
    {
        RemoveEnded();
        if (soundEnum == SoundEnum.Bombership || soundEnum == SoundEnum.Shootership ||
            soundEnum == SoundEnum.Transportship)
        {
            PlaySound(soundEnum);
        }
        else if (mRunningSounds.Count < SoundLimit)
        {
            // removing pan as it works only in one direction. i tried though
            pan *= 0;
            var instance = mLoadedSounds[soundEnum].CreateInstance();
            if (mSoundVolume <= 0)
            {
                mSoundVolume = 0;
            }

            if (mSoundVolume >= 1)
            {
                mSoundVolume = 1;
            }
            instance.Volume = mSoundVolume;
            if (mSoundMuted)
            {
                instance.Volume = 0;
            }

            if (-1.0 <= pan && pan <= 1.0)
            {
                instance.Pan = pan;
            }

            mRunningSounds.Add(instance);
            instance.Play();
        }
    }

    internal void PlayPitchedSound(SoundEnum soundEnum, float pitch)
    {
        RemoveEnded();
        if (mRunningSounds.Count < SoundLimit)
        {
            var instance = mLoadedSounds[soundEnum].CreateInstance();
            if (mSoundVolume <= 0)
            {
                mSoundVolume = 0;
            }
            if (mSoundVolume >= 1)
            {
                mSoundVolume = 1;
            }
            instance.Volume = mSoundVolume;
            if (mSoundMuted)
            {
                instance.Volume = 0;
            }

            if (-1 <= pitch && pitch <= 1)
            {
                instance.Pitch = pitch;
            }

            mRunningSounds.Add(instance);
            instance.Play();
        }
    }

    // used in update
    private void RemoveEnded()
    {
        // Backwards loop to cover for moving indices
        for (var i = mRunningSounds.Count - 1; i >= 0; i--)
        {
            if (mRunningSounds[i].State == SoundState.Stopped)
            {
                mRunningSounds.RemoveAt(i);
            }
        }
    }

    //changes volume of all sounds to value between 1 and 0
    public void SetSoundVolume(float vol)
    {
        if (0.0 <= vol && vol <= 1.0)
        {
            this.mSoundVolume = vol;
        }
        else if (vol > 1)
        {
            mSoundVolume = 1.0f;
        }
        else
        {
            mSoundVolume = 0;
        }

        foreach (var sound in mRunningSounds)
        {
            if (sound != null && sound.State != SoundState.Stopped)
            {
                if (!mSoundMuted)
                {
                    sound.Volume = ScaleVolume(this.mSoundVolume);
                }
                else
                {
                    sound.Volume = 0;
                }
            }
        }
    }


    public void SetMusicVolume(float vol)
    {
        if (0.0 <= vol && vol <= 1.0)
        {
            this.mMusicVolume = vol;
        }
        else if (vol > 1.0)
        {
            mMusicVolume = 1.0f;
        }
        else
        {
            mMusicVolume = 0;
        }

        if (mCurrentMusic != null)
        {
            if (!mSoundMuted)
            {
                mCurrentMusic.Volume = ScaleVolume(this.mMusicVolume);
            }
            else
            {
                mCurrentMusic.Volume = 0;
            }
        }
    }


    private float ScaleVolume(float vol)
    {
        return (float)(Math.Pow(vol, 4));
    }

    // Relevant for gui?
    public float GetSoundVolume()
    {
        return this.mSoundVolume;
    }

    public float GetMusicVolume()
    {
        return this.mMusicVolume;
    }

    internal void AddIAudible(IAudible audible)
    {
        mAudibles.Add(audible);
    }

    internal void RemoveEntities()
    {
        foreach (var audible in mAudibles)
        {
            if (audible is Component or Animation)
            {
                RemoveIAudible(audible);
            }
        }
    }

    private void RemoveIAudible(IAudible audible)
    {
        mAudibles.Remove(audible);
    }

}