using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Util.Pool;

public static class SoundStore
{
    private static Dictionary<string, AudioClip> _audioClipContainer = new Dictionary<string, AudioClip>();
    private static ObjectPooler<DisposableSound> _soundPooler;

    public static AudioClip GetAudioClip(string name)
    {
        return _audioClipContainer[name];
    }
    
    public static DisposableSound CreateAudioPlayer()
    {
        GameObject newGameObject = new GameObject();
        AudioSource audioSource = newGameObject.AddComponent<AudioSource>();
        DisposableSound disposableSound = newGameObject.AddComponent<DisposableSound>();

        return disposableSound;
    }

    public static void PlayAudio(string name, Vector3 position)
    {
        DisposableSound disposableSound = _soundPooler.Get();
        AudioClip clip = GetAudioClip(name);
        disposableSound.clip = clip;
        disposableSound.gameObject.SetActive(true);
    }

    public static void ReleaseSound(DisposableSound sound)
    {
        _soundPooler.Release(sound);
        sound.gameObject.SetActive(false);
    }

    static SoundStore()
    {
        _soundPooler = new ObjectPooler<DisposableSound>(CreateAudioPlayer);

        // ================================================================================================================================================
        // 배경음 추가..
        // ================================================================================================================================================
        _audioClipContainer.Add("Title_Enviorment", Resources.Load<AudioClip>("Sounds/Enviorment/Stupid_Dancer"));
        _audioClipContainer.Add("Banpick_Enviorment", Resources.Load<AudioClip>("Sounds/Enviorment/StreetLove"));
        _audioClipContainer.Add("BattleStage_Enviorment", Resources.Load<AudioClip>("Sounds/Enviorment/Electronic001"));
        _audioClipContainer.Add("Dormitory_Enviorment", Resources.Load<AudioClip>("Sounds/Enviorment/InfiniteDoors"));

        // ================================================================================================================================================
        // 효과음 추가..
        // ================================================================================================================================================
        // 밴픽 관련..
        _audioClipContainer.Add("BanChampion", Resources.Load<AudioClip>("Sounds/SoundEffect/Spear Stab (Flesh) 1"));
        _audioClipContainer.Add("PickChampion", Resources.Load<AudioClip>("Sounds/SoundEffect/RELOAD_Pump_stereo"));

        // 검사..
        _audioClipContainer.Add("Swordman_Attack", Resources.Load<AudioClip>("Sounds/SoundEffect/Throwing Knife (Thrown) 3"));
        _audioClipContainer.Add("Swordman_Skill", Resources.Load<AudioClip>("Sounds/SoundEffect/Throwing Knife (Thrown) 5"));
        _audioClipContainer.Add("Swordman_Ultimate", Resources.Load<AudioClip>("Sounds/SoundEffect/Spear Stab (Flesh) 1"));

        // 몽크..
        _audioClipContainer.Add("Monk_Attack", Resources.Load<AudioClip>("Sounds/SoundEffect/Punch Impact (Flesh) 3"));
        _audioClipContainer.Add("Monk_Skill", Resources.Load<AudioClip>("Sounds/SoundEffect/MAGIC_SPELL_Morphing_Synth_Harp_Scales_Subtle_stereo"));
        _audioClipContainer.Add("Monk_Ultimate", Resources.Load<AudioClip>("Sounds/SoundEffect/MAGIC_SPELL_Shield_mono"));

        // 궁수..
        _audioClipContainer.Add("Archer_Skill", Resources.Load<AudioClip>("Sounds/SoundEffect/Bow string drawing fast 1_20"));
        _audioClipContainer.Add("Arrow", Resources.Load<AudioClip>("Sounds/SoundEffect/Arrow Flying Past 6"));

        // 유령..
        _audioClipContainer.Add("Ghost_Attack", Resources.Load<AudioClip>("Sounds/SoundEffect/Bloody stabs 9"));
        _audioClipContainer.Add("Ghost_Ultimate", Resources.Load<AudioClip>("Sounds/SoundEffect/Drinking 6_cut"));

        // 기사..
        _audioClipContainer.Add("knight_Attack", Resources.Load<AudioClip>("Sounds/SoundEffect/Swing 1"));
        _audioClipContainer.Add("knight_Skill", Resources.Load<AudioClip>("Sounds/SoundEffect/VOICE_Martial_Art_Shout_22_mono"));
        _audioClipContainer.Add("knight_Ultimate", Resources.Load<AudioClip>("Sounds/SoundEffect/Buff 4"));

        // 소총수..
        _audioClipContainer.Add("Bullet", Resources.Load<AudioClip>("Sounds/SoundEffect/FIREARM_Assault_Rifle_Model_01_Fire_Single_RR1_stereo"));
        _audioClipContainer.Add("FollowBullet", Resources.Load<AudioClip>("Sounds/SoundEffect/FIREARM_Sub_Machine_Gun_Model_01_Fire_Single_RR1_stereo"));
        _audioClipContainer.Add("GrenadeBomb", Resources.Load<AudioClip>("Sounds/SoundEffect/Earth Bolt 14"));
        _audioClipContainer.Add("Soldier_Reload", Resources.Load<AudioClip>("Sounds/SoundEffect/RELOAD_Pump_stereo"));

        // 화염 술사..
        _audioClipContainer.Add("Pyromancer_Attack", Resources.Load<AudioClip>("Sounds/SoundEffect/Fire bolt 17"));
        _audioClipContainer.Add("Pyromancer_Skill", Resources.Load<AudioClip>("Sounds/SoundEffect/MAGIC_SPELL_Flame_04_mono"));
        _audioClipContainer.Add("FireBall", Resources.Load<AudioClip>("Sounds/SoundEffect/FLYBY_Missile_01_Fast_mono"));

        // 성직자..
        _audioClipContainer.Add("Priest_Attack", Resources.Load<AudioClip>("Sounds/SoundEffect/MAGIC_SPELL_Morphing_Synth_Harp_Scales_Subtle_stereo"));
        _audioClipContainer.Add("Priest_Skill", Resources.Load<AudioClip>("Sounds/SoundEffect/MAGIC_SPELL_Shield_mono"));

        // 얼음 술사..
        _audioClipContainer.Add("IceBullet", Resources.Load<AudioClip>("Sounds/SoundEffect/Arrow Flying Past 6"));
        _audioClipContainer.Add("Freeze", Resources.Load<AudioClip>("Sounds/SoundEffect/Light  Ice Footsteps 3-trim"));

        // 도박사..
        _audioClipContainer.Add("Chip", Resources.Load<AudioClip>("Sounds/SoundEffect/FLYBY_Missile_01_Fast_mono"));
        _audioClipContainer.Add("RandomDice_Decide", Resources.Load<AudioClip>("Sounds/SoundEffect/Item purchase 5"));
        _audioClipContainer.Add("SlotMachine_Land", Resources.Load<AudioClip>("Sounds/SoundEffect/Body Fall 7"));
        _audioClipContainer.Add("SlotMachine_Spin", Resources.Load<AudioClip>("Sounds/SoundEffect/Prize wheel spin 1"));
        _audioClipContainer.Add("SlotMachine_End", Resources.Load<AudioClip>("Sounds/SoundEffect/Cash register 5"));

        // 마검사..
        _audioClipContainer.Add("Magicknight_Attack", Resources.Load<AudioClip>("Sounds/SoundEffect/Heavy Sword Swing 14"));
        _audioClipContainer.Add("Magicknight_Skill", Resources.Load<AudioClip>("Sounds/SoundEffect/Heavy Sword Swing 10"));
        _audioClipContainer.Add("Magicknight_UltimateStart", Resources.Load<AudioClip>("Sounds/SoundEffect/Sword Stab (Flesh) 1"));
        _audioClipContainer.Add("Magicknight_UltimateImpact", Resources.Load<AudioClip>("Sounds/SoundEffect/WHOOSH_Steam_Fast_02_mono"));

        // 닌자..
        _audioClipContainer.Add("Ninja_Attack", Resources.Load<AudioClip>("Sounds/SoundEffect/Dagger Swing 8"));
        _audioClipContainer.Add("Ninja_SkillStart", Resources.Load<AudioClip>("Sounds/SoundEffect/Body Drop 3"));
        _audioClipContainer.Add("Ninja_SkillEnd", Resources.Load<AudioClip>("Sounds/SoundEffect/Stone Impact 4"));
        _audioClipContainer.Add("Ninja_Ultimate", Resources.Load<AudioClip>("Sounds/SoundEffect/MAGIC_SPELL_Spawn_mono"));
    }
}