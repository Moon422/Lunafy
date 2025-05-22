export interface TotalUsersStatApiResponse {
    presentDataTill?: string
    presentUserCount: number
    previousDataTill?: string
    previousUserCount: number
    infiniteIncrement: boolean
    didUserIncremented?: boolean
    changePercentage: number
}

interface UserModel {
    firstname: string
    lastname: string
    username: string
    email: string
    isAdmin: boolean
    isArtist: boolean
}

export interface UserCreateModel extends UserModel { }

export interface UserEditModel extends UserModel {
    id: number
}

export interface UserCreateErrorModel {
    firstname: string | null
    lastname: string | null
    username: string | null
    email: string | null
}

export interface UserReadModel extends UserModel {
    id: number
    isInactive: boolean
    inactiveTill?: string
    requirePasswordReset: boolean
    lastLogin: string
    createdOn: string
    modifiedOn?: string
    deleted: boolean
    deletedOn?: string
    isEditable: boolean
}

interface ArtistModel {
    firstname: string
    lastname: string
    biography: string | null
    musicBrainzId: string | null
}

export interface ArtistCreateModel extends ArtistModel { }

export interface ArtistEditModel extends ArtistModel {
    id: number
}

export interface ArtistReadModel extends ArtistModel {
    id: number
    createdOn: string
    modifiedOn: string | null
    deleted: boolean
    deletedOn: string | null
    profilePicture: PictureModel | null
}

export interface ArtistCreateErrorModel {
    firstname: string | null
    lastname: string | null
    musicBrainzId: string | null
}

export interface PictureModel {
    id: number
    imageFile: string | null
    thumb64: string
    thumb128: string
    thumb256: string
    thumb512: string
    thumb1024: string
}