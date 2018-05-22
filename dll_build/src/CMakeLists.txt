FILE(GLOB SRC
    *.cpp
    *.h
    )
ADD_LIBRARY(StbImage SHARED
    ${SRC}
    stb_image.def
    )

