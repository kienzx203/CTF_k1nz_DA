import { Center, Divider, Stack, Text } from '@mantine/core'
import { FC } from 'react'
import { Copyright } from '@Components/Copyright'
import { FooterRender } from '@Components/FooterRender'
import { MainIcon } from '@Components/icon/MainIcon'
import { useIsMobile } from '@Utils/ThemeOverride'
import { useConfig } from '@Hooks/useConfig'
import classes from '@Styles/AppFooter.module.css'
import logoClasses from '@Styles/LogoHeader.module.css'

export const AppFooter: FC = () => {
  const { config } = useConfig()
  const isMobile = useIsMobile()

  return (
    <>
      <div className={classes.spacer} />
      <div className={classes.wrapper}>
        <Center mx={0} px={0} h="100%" style={{ width: "100%" }}>
          <Stack mx={0} px={0} w="100%" style={{ width: "100%" }}>
            {config.footerInfo && (
              <FooterRender source={config.footerInfo} />
            )}
          </Stack>
        </Center>
      </div>
    </>
  )
}
