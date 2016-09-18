This project compares and contrasts the standard downloadable FXCM data from QuantConnect.com vs. the supposedly identical (or at least near identical) data downloaded via the ForexConnect API (see the related [GetFxcmHistory](https://github.com/squideyes/GetFxcmHistory) project for details).   As the demo shows, the QuantConnect spreads are significantly higher than the API spreads ; 277% on average!

I'm not quite sure what may causing this, although an initial pass with the java-API based FxcmDataDownloader seemed to show a similar variance.  I'm guessing that the problem may be an account-configuration issue on the FXCM side although I have yet to hear back from FXCM support on this.