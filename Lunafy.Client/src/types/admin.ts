export interface TotalUsersStatApiResponse {
    presentDataTill?: string
    presentUserCount: number
    previousDataTill?: string
    previousUserCount: number
    infiniteIncrement: boolean
    didUserIncremented?: boolean
    changePercentage: number
}

export interface UserModel {
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

export interface ArtistModel {
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
}

export interface ArtistCreateErrorModel {
    firstname: string | null
    lastname: string | null
    musicBrainzId: string | null
}