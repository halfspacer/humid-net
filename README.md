# humid-net

A mid-level networking framework for Unity that aims to provide a clean and robust abstraction layer without making too many assumptions regarding the higher level implementation. 

## Get Started

The repo implements Epic Online Services and LiteNetLib as sample backends. The former is a larger networking framework whereas LiteNetLib is a small Reliable UDP library; Comparing the implementation of the two should give a good overview of the process of implementing your own backend using the framework.

>[!NOTE]
>Voice Chat (for EOS) was implemented outside of the overarching abstraction layer, primarily for testing and debugging purposes as I was figuring out how to map Epic's audio buffer to Unity's audio system for spatialization.
>If you use this framework for any other backend, you need to remove VoiceManager. I might finish the VoIP implementation properly at some point.
